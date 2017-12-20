using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public interface IChatPostFactory
    {
        IChatPost NewInstance(IRestService restService, Uri baseUri, string token, string teamId, Post post);
    }
}