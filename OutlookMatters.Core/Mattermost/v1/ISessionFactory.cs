using System;
using OutlookMatters.Core.Chat;

namespace OutlookMatters.Core.Mattermost.v1
{
    public interface ISessionFactory
    {
        ISession NewInstance(Uri url, string token, string userId);
    }
}