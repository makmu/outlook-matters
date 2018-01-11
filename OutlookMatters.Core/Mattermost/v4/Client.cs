using System;
using System.Linq;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public class Client : IClient
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IRestService _restService;
        private readonly IChatFactory _chatFactory;

        public Client(IAuthenticationService authenticationService, IRestService restService, IChatFactory chatFactory)
        {
            _authenticationService = authenticationService;
            _restService = restService;
            _chatFactory = chatFactory;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            string token;
            var baseUri = new Uri(url);
            _authenticationService.Login(baseUri, username, password, out token);
            var team = _restService.GetTeams(baseUri, token).SingleOrDefault(t => t.Name == teamId || t.Id == teamId);
            if (team == null)
            {
                throw new ChatException("Invalid Team Id in Settings!");
            }
            return _chatFactory.NewInstance(_restService, new Uri(url), token, team.Id);
        }
    }
}