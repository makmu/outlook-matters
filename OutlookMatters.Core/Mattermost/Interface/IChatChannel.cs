namespace OutlookMatters.Core.Mattermost.Interface
{
    public interface IChatChannel
    {
        string Id { get; }
        string Name { get; }
        void CreatePost(string message);
    }
}