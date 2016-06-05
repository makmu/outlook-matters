using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace OutlookMatters.Core.Mattermost.v3
{
    public class ChatFactoryImpl : IChatFactory, IChatChannelFactory, IChatPostFactory
    {
        public ISession NewInstance(IRestService restService, Uri uri, string token, string userId, string teamId)
        {
            return new SessionImpl(restService, uri, token, userId, teamId, this, this);
        }

        public IChatChannel NewInstance(IRestService restService, Uri baseUri, string token, string userId,
            string teamId, Channel channel)
        {
            return new ChatChannelImpl(restService, baseUri, token, userId, teamId, channel);
        }

        public IChatPost NewInstance(IRestService restService, Uri baseUri, string token, string teamId, Post post)
        {
            return new ChatPostImpl(restService, baseUri, token, teamId, post);
        }
    }
}