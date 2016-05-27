using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost.HttpImpl;
using OutlookMatters.Core.Mattermost.Interface;

namespace Test.OutlookMatters.Core.Mattermost.HttpImpl
{
    [TestFixture]
    public class HttpSessionTest
    {
        private const string TOKEN = "token";
        private const string USER_ID = "userId";
        private const string CHANNEL_ID = "channelId";
        private const string MESSAGE = "Hello World!";
        private const string POST_ID = "postId";
        private const string ROOT_ID = "rootId";

        [Test]
        public void CreatePost_UsesRestServiceCorrectly()
        {
            var baseUri = new Uri("http://localhost/");
            var post = new Post
            {
                Id = string.Empty,
                ChannelId = CHANNEL_ID,
                Message = MESSAGE,
                UserId = USER_ID,
                RootId = string.Empty
            };
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.CreatePost(baseUri, TOKEN, CHANNEL_ID, post));
            var sut = new HttpSession(baseUri, TOKEN, USER_ID, restService.Object);

            sut.CreatePost(CHANNEL_ID, MESSAGE);

            restService.VerifyAll();
        }

        [Test]
        public void CreatePost_UsesRestServiceCorrectly_IfRootIdIsProvided()
        {
            var baseUri = new Uri("http://localhost/");
            var post = new Post
            {
                Id = string.Empty,
                ChannelId = CHANNEL_ID,
                Message = MESSAGE,
                UserId = USER_ID,
                RootId = ROOT_ID
            };
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.CreatePost(baseUri, TOKEN, CHANNEL_ID, post));
            var sut = new HttpSession(baseUri, TOKEN, USER_ID, restService.Object);

            sut.CreatePost(CHANNEL_ID, MESSAGE, ROOT_ID);

            restService.VerifyAll();
        }

        [Test]
        public void GetRootPost_ReturnsRootPost_IfParentIsNotRootPost()
        {
            var baseUri = new Uri("http://localhost");
            var thread = new Thread
            {
                Posts = new Dictionary<string, Post>
                {
                    [POST_ID] = new Post {Id = POST_ID, RootId = ROOT_ID},
                    [ROOT_ID] = new Post {Id = ROOT_ID, RootId = string.Empty}
                }
            };
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.GetThreadOfPosts(baseUri, TOKEN, POST_ID)).Returns(thread);
            var sut = new HttpSession(baseUri, TOKEN, USER_ID, restService.Object);

            var result = sut.GetRootPost(POST_ID);

            Assert.That(result, Is.EqualTo(thread.Posts[ROOT_ID]));
        }

        [Test]
        public void FetchChannelList_ReturnsChannelListFromRestService()
        {
            var baseUri = new Uri("http://localhost/");
            var channelList = new ChannelList();
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.GetChannelList(baseUri, TOKEN)).Returns(channelList);
            var sut = new HttpSession(baseUri, TOKEN, USER_ID, restService.Object);

            var result = sut.FetchChannelList();

            Assert.That(result, Is.EqualTo(channelList));
        }
    }
}