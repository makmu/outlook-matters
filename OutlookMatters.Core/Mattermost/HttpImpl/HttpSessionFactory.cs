using System;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpSessionFactory : ISessionFactory, IChatPostFactory
    {
        private readonly IRestService _restService;

        public HttpSessionFactory(IRestService restService)
        {
            _restService = restService;
        }

        public ISession NewInstance(Uri url, string token, string userId)
        {
            return new HttpSession(url, token, userId, _restService, this);
        }

        public IChatPost NewInstance(Uri baseUri, string token, string userId, Post post)
        {
            return new ChatPostImpl(_restService, baseUri, token, userId, post);
        }
    }
}