using System;
using OutlookMatters.Core.Mattermost.v1.Interface;

namespace OutlookMatters.Core.Mattermost.v1
{
    public interface IRestService
    {
        User Login(Uri baseUri, Login login, out string token);
        void CreatePost(Uri baseUri, string token, string channelId, Post newPost);
        Thread GetThreadOfPosts(Uri baseUri, string token, string postId);
        ChannelList GetChannelList(Uri baseUri, string token);
    }
}