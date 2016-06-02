using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace OutlookMatters.Core.Mattermost.v3
{
    public interface IChatFactory
    {
        ISession NewInstance(IRestService restService, Uri uri, string token, string userId, string teamId);
    }
}