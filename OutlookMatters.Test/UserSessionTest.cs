using System;
using Moq;
using NUnit.Framework;
using OutlookMatters.Http;
using OutlookMatters.Mattermost;

namespace OutlookMatters.Test
{
    [TestFixture]
    public class UserSessionTest
    {
        [Test]
        public void CreatePost_PostsHttpRequest()
        {
            const string token = "token";
            const string userId = "userId";
            const string channelId = "channelId";
            const string message = "Hello World!";
            const string jsonPost =
                "{\"channel_id\":\"" + channelId + "\",\"message\":\"" + message + "\",\"user_id\":\"" + userId + "\"}";
            var baseUri = new Uri("http://localhost");
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Post(new Uri(baseUri, "api/v1/channels/" + channelId + "/create")))
                .Returns(httpRequest.Object);
            var classUnderTest = new UserSession(baseUri, token, userId, httpClient.Object);

            classUnderTest.CreatePost(channelId, message);

            httpRequest.Verify(x => x.WithContentType("text/json"));
            httpRequest.Verify(x => x.WithHeader("Authorization", "Bearer " + token));
            httpRequest.Verify(x => x.Send(jsonPost));
        }
    }
}
