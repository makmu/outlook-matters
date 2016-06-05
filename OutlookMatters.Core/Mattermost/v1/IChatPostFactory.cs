using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v1.Interface;

namespace OutlookMatters.Core.Mattermost.v1
{
    public interface IChatPostFactory
    {
        IChatPost NewInstance(Uri baseUri, string token, string userId, Post posts);
    }
}