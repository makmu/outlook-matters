using System;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpClient : IClient
    {
        private readonly IHttpClient _client;
        private readonly ISessionFactory _sessionFactory;

        public HttpClient(ISessionFactory sessionFactory, IHttpClient client)
        {
            _sessionFactory = sessionFactory;
            _client = client;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            try
            {
                var loginUrl = new Uri(new Uri(url), "api/v1/users/client");
                var login = new Login
                {
                    name = teamId,
                    email = username,
                    password = password
                };
                using (var response = _client.Request(loginUrl)
                    .WithContentType("text/json")
                    .Post(JsonConvert.SerializeObject(login)))
                {
                    var token = response.GetHeaderValue("Token");
                    var payload = response.GetPayload();
                    var user = JsonConvert.DeserializeObject<User>(payload);

                    return _sessionFactory.CreateSession(new Uri(url), token, user.id);
                }
            }
            catch (HttpException hex)
            {
                var errorJson = hex.Response.GetPayload();
                var error = JsonConvert.DeserializeObject<Interface.Error>(errorJson);
                throw new MattermostException(error);
            }
        }
    }
}