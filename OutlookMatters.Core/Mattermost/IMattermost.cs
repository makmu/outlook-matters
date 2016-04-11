using OutlookMatters.Core.Mattermost.Session;

namespace OutlookMatters.Core.Mattermost
{
    public interface IMattermost
    {
        ISession LoginByUsername(string url, string teamId, string username, string password);
    }
}