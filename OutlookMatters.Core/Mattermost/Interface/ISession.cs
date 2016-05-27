namespace OutlookMatters.Core.Mattermost.Interface
{
    public interface ISession
    {
        void CreatePost(string channelId, string message);
        IChatPost GetPost(string postId);
        ChannelList FetchChannelList();
    }
}