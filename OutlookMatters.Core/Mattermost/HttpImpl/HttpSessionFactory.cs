using System;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpSessionFactory : ISessionFactory
    {
        private readonly IRestService _restService;

        public HttpSessionFactory(IRestService restService)
        {
            _restService = restService;
        }

        public ISession CreateSession(Uri url, string token, string userId)
        {
            return new HttpSession(url, token, userId, _restService);
        }
    }
}