using System.Collections.Generic;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public interface ISession
    {
        IEnumerable<IChatChannel> GetChannels();
        IChatChannel GetChannel(string channelId);
        IChatPost GetPost(string postId);
    }
}