using OutlookMatters.Mattermost.DataObjects;

namespace OutlookMatters.Mattermost.Session
{
    public interface ISession
    {
        void CreatePost(string channelId, string message, string rootId = "");
        Post GetRootPost(string postId);
        ChannelList FetchChannelList();
    }
}