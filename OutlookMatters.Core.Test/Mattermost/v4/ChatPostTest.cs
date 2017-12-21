using System;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost.v4;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v4
{
    [TestFixture]
    class ChatPostTest
    {
        public const string MESSAGE = "message";
        public const string TEAM_ID = "rootPost";
        public const string POST_ID = "postId";
        public const string ROOT_ID = "rootPost";
        public const string TOKEN = "token";
        public const string CHANNEL_ID = "channelId";
        public Uri BaseUri = new Uri("http://localhost/");

        [Test]
        public void Reply_CreateNewPostForRoot_IfThisPostIsNotRoot()
        {
            var post = new Post { Id = POST_ID, ChannelId = CHANNEL_ID, RootId = ROOT_ID };
            var newPost = new Post
            {
                ChannelId = CHANNEL_ID,
                Message = MESSAGE,
                RootId = ROOT_ID
            };
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.CreatePost(BaseUri, TOKEN, newPost));
            var sut = new ChatPost(restService.Object, BaseUri, TOKEN, TEAM_ID, post);

            sut.Reply(MESSAGE);

            restService.VerifyAll();
        }

        [Test]
        public void Reply_CreatesNewPostWithThisPostAsRootPost_IfThisPostIsRoot()
        {
            var post = new Post { Id = POST_ID, ChannelId = CHANNEL_ID, RootId = string.Empty };
            var newPost = new Post
            {
                ChannelId = CHANNEL_ID,
                Message = MESSAGE,
                RootId = POST_ID
            };
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.CreatePost(BaseUri, TOKEN, newPost));
            var sut = new ChatPost(restService.Object, BaseUri, TOKEN, TEAM_ID, post);

            sut.Reply(MESSAGE);

            restService.VerifyAll();
        }
    }
}
