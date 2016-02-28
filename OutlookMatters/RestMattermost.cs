using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace OutlookMatters
{
    public class RestMattermost: IMattermost
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IHttpClient _client;

        public RestMattermost(ISessionFactory sessionFactory, IHttpClient _client)
        {
            _sessionFactory = sessionFactory;
            this._client = _client;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            var loginUrl = new Uri(new Uri(url), "api/v1/users/login");
            var login = new Login
            {
                name = teamId,
                email = username,
                password = password
            };
            var response = _client.Post(loginUrl)
                .WithContentType("text/json")
                .Send(JsonConvert.SerializeObject(login));

            var token = response.GetHeaderValue("Token");
            var payload = response.GetPayload();
            var user = JsonConvert.DeserializeObject<User>(payload);

            return _sessionFactory.CreateSession(new Uri(url), token, user.id);
        }
    }
}
