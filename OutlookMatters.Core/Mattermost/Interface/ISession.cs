using System;
using System.Collections.Generic;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public interface ISession
    {
        [Obsolete]
        ChannelList FetchChannelList();

        IEnumerable<IChatChannel> GetChannels();
        IChatChannel GetChannel(string channelId);
        IChatPost GetPost(string postId);
    }
}