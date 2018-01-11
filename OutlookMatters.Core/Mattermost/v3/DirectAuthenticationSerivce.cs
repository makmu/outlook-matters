using System;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace OutlookMatters.Core.Mattermost.v3
{
    public class DirectAuthenticationSerivce : IAuthenticationService
    {
        private readonly IRestService _restService;

        public DirectAuthenticationSerivce(IRestService restService)
        {
            _restService = restService;
        }

        public void Login(Uri baseUri, string username, string password, out string token)
        {
            var login = new Login
            {
                LoginId = username,
                Password = password,
                Token = string.Empty
            };

            _restService.Login(baseUri, login, out token);
        }
    }
}