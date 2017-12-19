using System;
using System.Linq;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public class Client : IClient
    {
        private readonly IRestService _restService;
        private readonly IChatFactory _chatFactory;

        public Client(IRestService restService, IChatFactory chatFactory)
        {
            _restService = restService;
            _chatFactory = chatFactory;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            string token;
            var login = new Login
            {
                LoginId = username,
                Password = password
            };
            var baseUri = new Uri(url);
            _restService.Login(baseUri, login, out token);
            var team = _restService.GetTeams(baseUri, token).SingleOrDefault(t => t.Name == teamId);
            if (team == null)
            {
                throw new ChatException("Invalid Team Id in Settings!");
            }
            return _chatFactory.NewInstance(_restService, new Uri(url), token, team.Id);
        }
    }
}