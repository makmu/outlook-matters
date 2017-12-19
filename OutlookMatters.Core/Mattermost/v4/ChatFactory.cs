using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public class ChatFactory : IChatFactory, IChatChannelFactory, IChatPostFactory
    {
        public ISession NewInstance(IRestService restService, Uri uri, string token, string userId, string teamId)
        {
            throw new NotImplementedException();
        }

        public IChatChannel NewInstance(IRestService restService, Uri baseUri, string token, string userId, Channel channel)
        {
            throw new NotImplementedException();
        }

        public IChatPost NewInstance(Uri baseUri, string token, string userId, Post posts)
        {
            throw new NotImplementedException();
        }
    }
}
