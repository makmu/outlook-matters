using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v1.Interface;

namespace OutlookMatters.Core.Mattermost.v1
{
    public interface IChatChannelFactory
    {
        IChatChannel NewInstance(IRestService restService, Uri baseUri, string token, string userId, Channel channel);
    }
}