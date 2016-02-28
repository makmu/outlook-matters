namespace OutlookMatters.Mattermost
{
    public interface IMattermost
    {
        ISession LoginByUsername(string url, string teamId, string username, string password);
    }
}
