using OutlookMatters.Mattermost.DataObjects;

namespace OutlookMatters.Mattermost.Session
{
    public interface ISession
    {
        ChannelList ChannelList { get; }
        void CreatePost(string channelId, string message, string rootId = "");
        Post GetPostById(string postId);
        ChannelList FetchChannelList();
    }
}