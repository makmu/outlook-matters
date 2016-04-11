using System;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public interface ISessionFactory
    {
        ISession CreateSession(Uri url, string token, string userId);
    }
}