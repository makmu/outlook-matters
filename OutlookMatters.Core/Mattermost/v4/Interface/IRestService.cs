using System;
using System.Collections.Generic;

namespace OutlookMatters.Core.Mattermost.v4.Interface
{
    public interface IRestService
    {
        void Login(Uri baseUri, Login login, out string token);

        IEnumerable<Team> GetTeams(Uri baseUri, string token);

        IEnumerable<Channel> GetChannels(Uri baseUri, string token, string teamId);

        void CreatePost(Uri baseUri, string token, Post newPost);

        Post GetPostById(Uri baseUri, string token, string postId);
    }
}