using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.HttpImpl;
using OutlookMatters.Core.Mattermost.Interface;

namespace Test.OutlookMatters.Core.Mattermost.HttpImpl
{
    [TestFixture]
    public class HttpSessionTest
    {
        private const string TOKEN = "token";
        private const string USER_ID = "userId";
        private const string CHANNEL1_ID = "channelId1";
        private const string CHANNEL1_NAME = "channelName1";
        private const string CHANNEL2_ID = "channelId2";
        private const string CHANNEL2_NAME = "channelName2";
        private const string MESSAGE = "Hello World!";
        private const string POST_ID = "postId";
        private const string ROOT_ID = "rootId";

        [Test]
        public void GetChannels_GetsChannelsFromRestAndCreateChannelObjects()
        {
            var baseUri = new Uri("http://localhost/");
            var channelList = new ChannelList
            {
                Channels = new List<Channel>
                {
                    new Channel
                    {
                        ChannelId = CHANNEL1_ID,
                        ChannelName = CHANNEL1_NAME
                    },
                    new Channel
                    {
                        ChannelId = CHANNEL2_ID,
                        ChannelName = CHANNEL2_NAME
                    }
                }
            };
            var list = new List<IChatChannel> {Mock.Of<IChatChannel>(), Mock.Of<IChatChannel>()};
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.GetChannelList(baseUri, TOKEN)).Returns(channelList);
            var channelFactory = new Mock<IChatChannelFactory>();
            channelFactory.Setup(
                x => x.NewInstance(restService.Object, baseUri, TOKEN, USER_ID, channelList.Channels[0]))
                .Returns(list[0]);
            channelFactory.Setup(
                x => x.NewInstance(restService.Object, baseUri, TOKEN, USER_ID, channelList.Channels[1]))
                .Returns(list[1]);
            var sut = new HttpSession(restService.Object, baseUri,
                TOKEN, USER_ID, Mock.Of<IChatPostFactory>(), channelFactory.Object);

            var result = sut.GetChannels();

            Assert.True(result.SequenceEqual(list));
        }

        [Test]
        public void GetChannel_GetsChannelsFromRestAndCreateChannelObjects()
        {
            var baseUri = new Uri("http://localhost/");
            var channel = new Channel
            {
                ChannelId = CHANNEL1_ID
            };
            var channelObj = new Mock<IChatChannel>();
            var restService = Mock.Of<IRestService>();
            var channelFactory = new Mock<IChatChannelFactory>();
            channelFactory.Setup(
                x => x.NewInstance(restService, baseUri, TOKEN, USER_ID, channel))
                .Returns(channelObj.Object);
            var sut = new HttpSession(restService, baseUri,
                TOKEN, USER_ID, Mock.Of<IChatPostFactory>(), channelFactory.Object);

            var result = sut.GetChannel(CHANNEL1_ID);

            Assert.That(result, Is.EqualTo(channelObj.Object));
        }

        [Test]
        public void GetPost_CreatesChatPostCorrectly()
        {
            var baseUri = new Uri("http://localhost");
            var thread = new Thread
            {
                Order = new[] {POST_ID, ROOT_ID},
                Posts = new Dictionary<string, Post>()
            };
            thread.Posts[POST_ID] = new Post {Id = POST_ID, RootId = ROOT_ID};
            thread.Posts[ROOT_ID] = new Post {Id = ROOT_ID, RootId = string.Empty};
            var restService = new Mock<IRestService>();
            var factory = new Mock<IChatPostFactory>();
            var chatPost = new Mock<IChatPost>();
            restService.Setup(x => x.GetThreadOfPosts(baseUri, TOKEN, POST_ID)).Returns(thread);
            factory.Setup(x => x.NewInstance(baseUri, TOKEN, USER_ID, thread.Posts[POST_ID])).Returns(chatPost.Object);
            var sut = new HttpSession(restService.Object, baseUri,
                TOKEN, USER_ID, factory.Object, Mock.Of<IChatChannelFactory>());

            var result = sut.GetPost(POST_ID);

            Assert.That(result, Is.EqualTo(chatPost.Object));
        }

        [Test]
        public void FetchChannelList_ReturnsChannelListFromRestService()
        {
            var baseUri = new Uri("http://localhost/");
            var channelList = new ChannelList();
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.GetChannelList(baseUri, TOKEN)).Returns(channelList);
            var sut = new HttpSession(restService.Object, baseUri,
                TOKEN, USER_ID, Mock.Of<IChatPostFactory>(), Mock.Of<IChatChannelFactory>());

            var result = sut.FetchChannelList();

            Assert.That(result, Is.EqualTo(channelList));
        }
    }
}