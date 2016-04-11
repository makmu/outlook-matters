using System;

namespace OutlookMatters.Core.Mattermost.Session
{
    public interface ISessionFactory
    {
        ISession CreateSession(Uri url, string token, string userId);
    }
}