using System;
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
            throw new NotImplementedException();
        }
    }
}
