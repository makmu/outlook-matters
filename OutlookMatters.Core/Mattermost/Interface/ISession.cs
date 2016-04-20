namespace OutlookMatters.Core.Mattermost.Interface
{
    public interface ISession
    {
        Payload CreatePost(string channelId, string message, string rootId = "");
        Post GetRootPost(string postId);
        ChannelList FetchChannelList();
    }
}