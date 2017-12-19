using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public interface IChatFactory
    {
        ISession NewInstance(IRestService restService, Uri uri, string token, string teamId);
    }
}
