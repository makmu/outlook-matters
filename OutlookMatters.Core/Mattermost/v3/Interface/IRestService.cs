using System;

namespace OutlookMatters.Core.Mattermost.v3.Interface
{
    public interface IRestService
    {
        User Login(Uri baseUri, Login login, out string token);
        ChannelList GetChannelList(Uri uri, string token, string teamId);
    }
}