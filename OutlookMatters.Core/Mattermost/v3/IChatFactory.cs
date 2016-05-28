using System;
using OutlookMatters.Core.Chat;

namespace OutlookMatters.Core.Mattermost.v3
{
    public interface IChatFactory
    {
        ISession NewInstance(Uri uri, string token, string userId);
    }
}