using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost.v4;
using OutlookMatters.Core.Mattermost.v4.Interface;
using OutlookMatters.Core.Settings;

namespace Test.OutlookMatters.Core.Mattermost.v4
{
    [TestFixture]
    public class ChatChannelTest
    {
        public const string TOKEN = "token";
        public const string CHANNEL_ID = "channelId";
        public const string CHANNEL_NAME = "channelName";
        public const string TEAM_GUID = "teamGuid";
        public const string MESSAGE = "message";
        public const ChannelType CHANNEL_TYPE = ChannelType.Public;
        public const ChannelTypeSetting CHANNEL_TYPE_SETTING = ChannelTypeSetting.Public;

        [Test]
        public void CreatePost_UsesRestServiceToCreatePost()
        {
            var baseUri = new Uri("http://localhost/");
            var post = new Post
            {
                ChannelId = CHANNEL_ID,
                Message = MESSAGE
            };
            var channel = new Channel {Id = CHANNEL_ID};
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.CreatePost(baseUri, TOKEN, post));
            var sut = new ChatChannel(restService.Object, baseUri, TOKEN, TEAM_GUID, channel);

            sut.CreatePost(MESSAGE);

            restService.VerifyAll();
        }

        [Test]
        public void ToSetting_ReturnsChannelId()
        {
            var baseUri = new Uri("http://localhost/");
            var channel = new Channel {Id = CHANNEL_ID, Name = CHANNEL_NAME, Type = CHANNEL_TYPE};
            var sut = new ChatChannel(Mock.Of<IRestService>(), baseUri, TOKEN, TEAM_GUID, channel);

            var result = sut.ToSetting();

            result.ChannelId.ShouldBeEquivalentTo(CHANNEL_ID);
            result.ChannelName.ShouldBeEquivalentTo(CHANNEL_NAME);
            result.Type.ShouldBeEquivalentTo(CHANNEL_TYPE_SETTING);
        }
    }
}
