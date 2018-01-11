using System;
using System.Linq;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace OutlookMatters.Core.Mattermost.v3
{
    public class ClientImpl : IClient
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IRestService _restService;
        private readonly IChatFactory _factory;

        public ClientImpl(IAuthenticationService authenticationService, IRestService restService, IChatFactory factory)
        {
            _authenticationService = authenticationService;
            _restService = restService;
            _factory = factory;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            string token;
            var baseUri = new Uri(url);
            _authenticationService.Login(baseUri,username,password,out token);
            var initialLoad = _restService.GetInitialLoad(baseUri, token);
            var team = initialLoad.Teams.SingleOrDefault(y => y.Name == teamId);
            if (team == null)
            {
                throw new ChatException("Invalid Team Id in Settings!");
            }
            return _factory.NewInstance(_restService, new Uri(url), token, team.Id);
        }
    }
}