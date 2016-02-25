namespace OutlookMatters
{
    public interface IMattermost
    {
        ISession LoginByUsername(string url, string teamId, string username, string password);
    }
}
