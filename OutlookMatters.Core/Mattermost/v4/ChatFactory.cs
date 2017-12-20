using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public class ChatFactory : IChatFactory, IChatChannelFactory, IChatPostFactory
    {
        public ISession NewInstance(IRestService restService, Uri baseUri, string token, string teamId)
        {
            return new Session(restService, baseUri, token, teamId, this, this);
        }

        public IChatChannel NewInstance(IRestService restService, Uri baseUri, string token, string teamId,
            Channel channel)
        {
            return new ChatChannel(restService, baseUri, token, teamId, channel);
        }

        public IChatPost NewInstance(Uri baseUri, string token, string userId, Post posts)
        {
            throw new NotImplementedException();
        }
    }
}
