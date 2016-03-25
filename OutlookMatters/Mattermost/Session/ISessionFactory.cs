using System;

namespace OutlookMatters.Mattermost.Session
{
    public interface ISessionFactory
    {
        ISession CreateSession(Uri url, string token, string userId);
    }
}