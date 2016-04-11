using OutlookMatters.Core.Mattermost.DataObjects;

namespace OutlookMatters.Core.Mattermost.Session
{
    public interface ISession
    {
        void CreatePost(string channelId, string message, string rootId = "");
        Post GetRootPost(string postId);
        ChannelList FetchChannelList();
    }
}