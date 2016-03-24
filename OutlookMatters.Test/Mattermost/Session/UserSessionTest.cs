using System;
using Moq;
using NUnit.Framework;
using OutlookMatters.Http;
using OutlookMatters.Mattermost.Session;

namespace OutlookMatters.Test.Mattermost.Session
{
    [TestFixture]
    public class UserSessionTest
    {
        private const string Token = "token";
        private const string UserId = "userId";
        private const string ChannelId = "channelId";
        private const string Message = "Hello World!";
        private const string RootId = "rootId";

        [Test]
        public void CreatePost_PostsHttpRequest()
        {
            const string jsonPost =
                @"{""channel_id"":""channelId"",""message"":""Hello World!"",""user_id"":""userId"",""root_id"":""""}";
            var httpRequest = new Mock<IHttpRequest>();
            var classUnderTest = SetupUserSessionForCreatingPosts(httpRequest);

            classUnderTest.CreatePost(ChannelId, Message);

            httpRequest.Verify(x => x.WithContentType("text/json"));
            httpRequest.Verify(x => x.WithHeader("Authorization", "Bearer " + Token));
            httpRequest.Verify(x => x.PostAndForget(jsonPost));
        }

        [Test]
        public void CreatePost_PostsHttpRequestWithRootId_IfRootIdProvided()
        {
            const string jsonPost =
                @"{""channel_id"":""channelId"",""message"":""Hello World!"",""user_id"":""userId"",""root_id"":""rootId""}";
            var httpRequest = new Mock<IHttpRequest>();
            var classUnderTest = SetupUserSessionForCreatingPosts(httpRequest);

            classUnderTest.CreatePost(ChannelId, Message, RootId);

            httpRequest.Verify(x => x.WithContentType("text/json"));
            httpRequest.Verify(x => x.WithHeader("Authorization", "Bearer " + Token));
            httpRequest.Verify(x => x.PostAndForget(jsonPost));
        }

        private static UserSession SetupUserSessionForCreatingPosts(Mock<IHttpRequest> httpRequest)
        {
            var baseUri = new Uri("http://localhost");
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, "api/v1/channels/" + ChannelId + "/create")))
                .Returns(httpRequest.Object);
            var classUnderTest = new UserSession(baseUri, Token, UserId, httpClient.Object);
            return classUnderTest;
        }

    }
}
