using System;

namespace OutlookMatters
{
    public interface ISessionFactory
    {
        ISession CreateSession(Uri url, string token, string userId);
    }
}
