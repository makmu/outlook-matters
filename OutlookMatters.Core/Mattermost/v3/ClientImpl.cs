using System;
using System.Linq;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace OutlookMatters.Core.Mattermost.v3
{
    public class ClientImpl : IClient
    {
        private readonly IRestService _restService;
        private readonly IChatFactory _factory;

        public ClientImpl(IRestService restService, IChatFactory factory)
        {
            _restService = restService;
            _factory = factory;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            string token;
            var login = new Login
            {
                LoginId = username,
                Password = password,
                Token = string.Empty
            };
            var baseUri = new Uri(url);
            var user = _restService.Login(baseUri, login, out token);
            var initialLoad = _restService.GetInitialLoad(baseUri, token);
            var team = initialLoad.Teams.SingleOrDefault(y => y.Name == teamId);
            if (team == null)
            {
                throw new ChatException("Invalid Team Id in Settings!");
            }
            return _factory.NewInstance(_restService, new Uri(url), token, user.Id, team.Id);
        }
    }
}