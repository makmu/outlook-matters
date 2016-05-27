using System;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpSessionFactory : ISessionFactory, IChatPostFactory, IChatChannelFactory
    {
        private readonly IRestService _restService;

        public HttpSessionFactory(IRestService restService)
        {
            _restService = restService;
        }

        public ISession NewInstance(Uri url, string token, string userId)
        {
            return new HttpSession(_restService, url, token, userId, this, this);
        }

        public IChatPost NewInstance(Uri baseUri, string token, string userId, Post post)
        {
            return new ChatPostImpl(_restService, baseUri, token, userId, post);
        }

        public IChatChannel NewInstance(IRestService restService, Uri baseUri, string token, string userId,
            Channel channel)
        {
            return new ChatChannelImpl(restService, baseUri, token, userId, channel);
        }
    }
}