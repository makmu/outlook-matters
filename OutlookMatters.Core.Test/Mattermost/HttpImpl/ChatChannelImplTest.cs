using System;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost.HttpImpl;
using OutlookMatters.Core.Mattermost.Interface;

namespace Test.OutlookMatters.Core.Mattermost.HttpImpl
{
    [TestFixture]
    public class ChatChannelImplTest
    {
        public const string MESSAGE = "message";
        public const string TOKEN = "token";
        public const string USER_ID = "userId";
        public const string CHANNEL_ID = "channelId";
        public const string CHANNEL_NAME = "channelName";

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
        public void Id_ReturnsChannelId()
        {
            var baseUri = new Uri("http://localhost/");
            var channel = new Channel {ChannelId = CHANNEL_ID};
            var sut = new ChatChannelImpl(Mock.Of<IRestService>(), baseUri, TOKEN, USER_ID, channel);

            var result = sut.Id;

            Assert.That(result, Is.EqualTo(CHANNEL_ID));
        }

        [Test]
        public void Name_ReturnsChannelName()
        {
            var baseUri = new Uri("http://localhost/");
            var channel = new Channel {ChannelName = CHANNEL_NAME};
            var sut = new ChatChannelImpl(Mock.Of<IRestService>(), baseUri, TOKEN, USER_ID, channel);

            var result = sut.Name;

            Assert.That(result, Is.EqualTo(CHANNEL_NAME));
        }
    }
}