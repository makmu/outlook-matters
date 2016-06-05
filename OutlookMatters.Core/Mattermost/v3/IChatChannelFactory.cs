using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace OutlookMatters.Core.Mattermost.v3
{
    public interface IChatChannelFactory
    {
        IChatChannel NewInstance(IRestService restService, Uri baseUri, string token, string userId, string teamId,
            Channel channel);
    }
}