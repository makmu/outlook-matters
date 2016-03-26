namespace OutlookMatters.Mattermost.Session
{
    public interface ISession
    {
        Channels ChannelList { get; }
        void CreatePost(string channelId, string message, string rootId = "");
        Post GetPostById(string postId);
        void FetchChannelList();
    }
}