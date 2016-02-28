using System;

namespace OutlookMatters.Mattermost
{
    public interface ISessionFactory
    {
        ISession CreateSession(Uri url, string token, string userId);
    }
}
