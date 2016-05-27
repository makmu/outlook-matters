using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public interface ISessionFactory
    {
        ISession NewInstance(Uri url, string token, string userId);
    }
}