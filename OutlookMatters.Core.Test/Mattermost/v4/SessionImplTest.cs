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
    }
}