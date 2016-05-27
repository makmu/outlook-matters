using System;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost;
using OutlookMatters.Core.Mattermost.Interface;
using OutlookMatters.Core.Settings;

namespace Test.OutlookMatters.Core.Mattermost
{
    [TestFixture]
    public class ChatChannelImplTest
    {
        public const string MESSAGE = "message";
        public const string TOKEN = "token";
        public const string USER_ID = "userId";
        public const string CHANNEL_ID = "channelId";
        public const string CHANNEL_NAME = "channelName";
        public const ChannelType CHANNEL_TYPE = ChannelType.Public;
        public const ChannelTypeSetting CHANNEL_TYPE_SETTING = ChannelTypeSetting.Public;

        [Test]
        public void CreatePost_UsesRestServiceToCreatePost()
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
            var channel = new Channel {ChannelId = CHANNEL_ID};
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.CreatePost(baseUri, TOKEN, CHANNEL_ID, post));
            var sut = new ChatChannelImpl(restService.Object, baseUri, TOKEN, USER_ID, channel);

            sut.CreatePost(MESSAGE);

            restService.VerifyAll();
        }

        [Test]
        public void ToSetting_ReturnsChannelId()
        {
            var baseUri = new Uri("http://localhost/");
            var channel = new Channel {ChannelId = CHANNEL_ID, ChannelName = CHANNEL_NAME, Type = CHANNEL_TYPE};
            var sut = new ChatChannelImpl(Mock.Of<IRestService>(), baseUri, TOKEN, USER_ID, channel);

            var result = sut.ToSetting();

            Assert.That(result.ChannelId, Is.EqualTo(CHANNEL_ID));
            Assert.That(result.ChannelName, Is.EqualTo(CHANNEL_NAME));
            Assert.That(result.Type, Is.EqualTo(CHANNEL_TYPE_SETTING));
        }
    }
}