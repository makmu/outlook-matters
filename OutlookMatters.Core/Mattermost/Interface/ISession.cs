using System;
using System.Collections.Generic;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public interface ISession
    {
        [Obsolete]
        void CreatePost(string channelId, string message);

        [Obsolete]
        ChannelList FetchChannelList();

        IEnumerable<IChatChannel> GetChannels();
        IChatChannel GetChannel(string channelId);
        IChatPost GetPost(string postId);
    }
}