using System;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.DataObjects;
using OutlookMatters.Core.Mattermost.Session;

namespace OutlookMatters.Core.Mattermost
{
    public class RestMattermost : IMattermost
    {
        private readonly IHttpClient _client;
        private readonly ISessionFactory _sessionFactory;

        public RestMattermost(ISessionFactory sessionFactory, IHttpClient client)
        {
            _sessionFactory = sessionFactory;
            _client = client;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            try
            {
                var loginUrl = new Uri(new Uri(url), "api/v1/users/login");
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
                var error = JsonConvert.DeserializeObject<DataObjects.Error>(errorJson);
                throw new MattermostException(error);
            }
        }
    }
}