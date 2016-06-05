using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v3
{
    [TestFixture]
    public class SessionImplTest
    {
        private const string TOKEN = "token";
        private const string USER_ID = "userId";
        private const string TEAM_GUID = "teamGuid";
        private const string CHANNEL1_ID = "channelId1";
        private const string CHANNEL1_NAME = "channelName1";
        private const string CHANNEL2_ID = "channelId2";
        private const string CHANNEL2_NAME = "channelName2";
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
            restService.Setup(x => x.GetChannelList(baseUri, TOKEN, TEAM_GUID)).Returns(channelList);
            var channelFactory = new Mock<IChatChannelFactory>();
            channelFactory.Setup(
                x => x.NewInstance(restService.Object, baseUri, TOKEN, USER_ID, TEAM_GUID, channelList.Channels[0]))
                .Returns(list[0]);
            channelFactory.Setup(
                x => x.NewInstance(restService.Object, baseUri, TOKEN, USER_ID, TEAM_GUID, channelList.Channels[1]))
                .Returns(list[1]);
            var sut = new SessionImpl(restService.Object, baseUri,
                TOKEN, USER_ID, TEAM_GUID, channelFactory.Object, Mock.Of<IChatPostFactory>());

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
                x => x.NewInstance(restService, baseUri, TOKEN, USER_ID, TEAM_GUID, channel))
                .Returns(channelObj.Object);
            var sut = new SessionImpl(restService, baseUri,
                TOKEN, USER_ID, TEAM_GUID, channelFactory.Object, Mock.Of<IChatPostFactory>());

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
            var chatPost = new Mock<IChatPost>();
            var channelFactory = new Mock<IChatPostFactory>();
            channelFactory.Setup(
                x => x.NewInstance(restService.Object, baseUri, TOKEN, TEAM_GUID, thread.Posts[POST_ID]))
                .Returns(chatPost.Object);
            restService.Setup(x => x.GetPostById(baseUri, TOKEN, TEAM_GUID, POST_ID)).Returns(thread);
            var sut = new SessionImpl(restService.Object, baseUri,
                TOKEN, USER_ID, TEAM_GUID, Mock.Of<IChatChannelFactory>(), channelFactory.Object);

            var result = sut.GetPost(POST_ID);

            Assert.That(result, Is.EqualTo(chatPost.Object));
        }
    }
}