namespace OutlookMatters.Mattermost
{
    public interface ISession
    {
        void CreatePost(string channelId, string message);
    }
}
