using System;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public interface IRestService
    {
        User Login(Uri baseUri, Login login, out string token);
        Post CreatePost(Uri baseUri, string token, Post newPost);
        Thread GetPostsThread(Uri baseUri, string token, string postId);
        ChannelList GetChannelList(Uri baseUri, string token);
    }
}