using System;
using System.Collections.Generic;

namespace OutlookMatters.Core.Mattermost.v3.Interface
{
    public interface IRestService
    {
        User Login(Uri baseUri, Login login, out string token);
        InitialLoad GetInitialLoad(Uri baseUri, string token);
        IEnumerable<Channel> GetChannelList(Uri uri, string token, string teamGuid);
        void CreatePost(Uri baseUri, string token, string channelId, string teamGuid, Post newPost);
        Thread GetPostById(Uri baseUri, string token, string teamGuid, string postId);
    }
}