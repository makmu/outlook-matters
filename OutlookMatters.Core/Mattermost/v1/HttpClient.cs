using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v1.Interface;

namespace OutlookMatters.Core.Mattermost.v1
{
    public class HttpClient : IClient
    {
        private readonly IRestService _restService;
        private readonly ISessionFactory _sessionFactory;

        public HttpClient(ISessionFactory sessionFactory, IRestService restService)
        {
            _sessionFactory = sessionFactory;
            _restService = restService;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            var login = new Login
            {
                Name = teamId,
                Email = username,
                Password = password
            };
            string token;
            var user = _restService.Login(new Uri(url), login, out token);
            return _sessionFactory.NewInstance(new Uri(url), token, user.Id);
        }
    }
}