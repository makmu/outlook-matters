using System;
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
            var login = new Login {LoginId = username, Password = password, Token = string.Empty};
            var user = _restService.Login(new Uri(url), login, out token);
            return _factory.NewInstance(new Uri(url), token, user.Id);
        }
    }
}