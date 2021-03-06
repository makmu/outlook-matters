﻿using System;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost.v3;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v3
{
    [TestFixture]
    public class ChatPostImplTest
    {
        public const string MESSAGE = "message";
        public const string USER_ID = "teamId";
        public const string TEAM_ID = "rootPost";
        public const string POST_ID = "postId";
        public const string ROOT_ID = "rootPost";
        public const string TOKEN = "token";
        public const string CHANNEL_ID = "channelId";
        public Uri BaseUri = new Uri("http://localhost/");

        [Test]
        public void Reply_CreateNewPostForRoot_IfThisPostIsNotRoot()
        {
            var post = new Post {Id = POST_ID, ChannelId = CHANNEL_ID, RootId = ROOT_ID};
            var newPost = new Post
            {
                Id = string.Empty,
                ChannelId = CHANNEL_ID,
                Message = MESSAGE,
                RootId = ROOT_ID
            };
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.CreatePost(BaseUri, TOKEN, CHANNEL_ID, TEAM_ID, newPost));
            var sut = new ChatPostImpl(restService.Object, BaseUri, TOKEN, TEAM_ID, post);

            sut.Reply(MESSAGE);

            restService.VerifyAll();
        }

        [Test]
        public void Reply_CreatesNewPostWithThisPostAsRootPost_IfThisPostIsRoot()
        {
            var post = new Post {Id = POST_ID, ChannelId = CHANNEL_ID, RootId = string.Empty};
            var newPost = new Post
            {
                Id = string.Empty,
                ChannelId = CHANNEL_ID,
                Message = MESSAGE,
                RootId = POST_ID
            };
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.CreatePost(BaseUri, TOKEN, CHANNEL_ID, TEAM_ID, newPost));
            var sut = new ChatPostImpl(restService.Object, BaseUri, TOKEN, TEAM_ID, post);

            sut.Reply(MESSAGE);

            restService.VerifyAll();
        }
    }
}