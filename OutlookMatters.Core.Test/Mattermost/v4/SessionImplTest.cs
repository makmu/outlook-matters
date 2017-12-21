using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v4
{
    [TestFixture]
    public class SessionImplTest
    {
        private readonly string TOKEN = "token";
        private const string CHANNEL_ID = "channel id";
        private const string CHANNEL_NAME = "channel name";
        private const string CHANNEL_ID_2 = "channel id 2";
        private const string CHANNEL_NAME_2 = "channel name 2";
        private const string TEAM_ID = "team id";
        private const string POST_ID = "post id";
        private const string ROOT_ID = "root id";

        [Test]
        public void GetChannels_GetsChannelsFromRestAndCreateChannelObjects()
        {
            var baseUri = new Uri("http://localhost/");
            var channelList = new List<Channel>
            {
                new Channel
                {
                    Id = CHANNEL_ID,
                    Name = CHANNEL_NAME
                },
                new Channel
                {
                    Id = CHANNEL_ID_2,
                    Name = CHANNEL_NAME_2
                }
            };
            var list = new List<IChatChannel> { Mock.Of<IChatChannel>(), Mock.Of<IChatChannel>() };
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.GetChannels(baseUri, TOKEN, TEAM_ID)).Returns(channelList);
            var channelFactory = new Mock<IChatChannelFactory>();
            channelFactory.Setup(
                    x => x.NewInstance(restService.Object, baseUri, TOKEN, TEAM_ID, channelList[0]))
                .Returns(list[0]);
            channelFactory.Setup(
                    x => x.NewInstance(restService.Object, baseUri, TOKEN, TEAM_ID, channelList[1]))
                .Returns(list[1]);
            var sut = new SessionImpl(restService.Object, baseUri,
                TOKEN, TEAM_ID, channelFactory.Object, Mock.Of<IChatPostFactory>());

            var result = sut.GetChannels();

            result.ShouldBeEquivalentTo(channelList);
        }

        [Test]
        public void GetChannel_GetsChannelsFromRestAndCreateChannelObjects()
        {
            var baseUri = new Uri("http://localhost/");
            var channel = new Channel
            {
                Id = CHANNEL_ID,
            };
            var channelObj = new Mock<IChatChannel>();
            var restService = Mock.Of<IRestService>();
            var channelFactory = new Mock<IChatChannelFactory>();
            channelFactory.Setup(
                    x => x.NewInstance(restService, baseUri, TOKEN, TEAM_ID, channel))
                .Returns(channelObj.Object);
            var sut = new SessionImpl(restService, baseUri,
                TOKEN, TEAM_ID, channelFactory.Object, Mock.Of<IChatPostFactory>());

            var result = sut.GetChannel(CHANNEL_ID);

            result.ShouldBeEquivalentTo(channelObj.Object);
        }

        [Test]
        public void GetPost_CreatesChatPostCorrectly()
        {
            var baseUri = new Uri("http://localhost");
            
            var post = new Post {Id = POST_ID, RootId = ROOT_ID};
            var restService = new Mock<IRestService>();
            var chatPost = new Mock<IChatPost>();
            var channelFactory = new Mock<IChatPostFactory>();
            channelFactory.Setup(
                    x => x.NewInstance(restService.Object, baseUri, TOKEN, TEAM_ID, post))
                .Returns(chatPost.Object);
            restService.Setup(x => x.GetPostById(baseUri, TOKEN, POST_ID)).Returns(post);
            var sut = new SessionImpl(restService.Object, baseUri,
                TOKEN, TEAM_ID, Mock.Of<IChatChannelFactory>(), channelFactory.Object);

            var result = sut.GetPost(POST_ID);

            result.ShouldBeEquivalentTo(chatPost.Object);
        }
    }
}