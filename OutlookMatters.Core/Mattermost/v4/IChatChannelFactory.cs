using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public interface IChatChannelFactory
    {
        IChatChannel NewInstance(IRestService restService, Uri baseUri, string token, string userId, Channel channel);
    }
}