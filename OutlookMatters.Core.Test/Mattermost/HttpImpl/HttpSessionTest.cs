using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Http;
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
            var sut = new HttpSession(baseUri, TOKEN, USER_ID, null, restService.Object);

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
            var sut = new HttpSession(baseUri, TOKEN, USER_ID, null, restService.Object);

            sut.CreatePost(CHANNEL_ID, MESSAGE, ROOT_ID);

            restService.VerifyAll();
        }

        [Test]
        public void FetchChannelList_ReturnsChannelList()
        {
            const string channelName = "FunnyChannelName";
            const string channelId = "1234";
            const string channelType = "O";
            const ChannelType expectedChannelType = ChannelType.Public;
            const string jsonResponse =
                "{\"channels\":[{\"id\":\"" + channelId +
                "\",\"create_at\":1458911668852,\"update_at\":1458911668852,\"delete_at\":0,\"type\":\"" + channelType +
                "\",\"display_name\":\"" + channelName + "\"}]}";
            var httpRequest = new Mock<IHttpRequest>();
            var classUnderTest = SetupUserSessionForFetchingChannelList(httpRequest, jsonResponse);

            var result = classUnderTest.FetchChannelList();

            result.Channels[0].ChannelId.Should().Be(channelId);
            result.Channels[0].ChannelName.Should().Be(channelName);
            result.Channels[0].Type.Should().Be(expectedChannelType);
        }

        [Test]
        public void FetchChannelList_ThrowsMattermostException_IfHttpExceptionIsThrown()
        {
            const string errorMessage = "error message";
            const string detailedError = "detailed error";
            const string jsonResponse =
                "{\"message\":\"" + errorMessage + "\",\"detailed_error\":\"" + detailedError + "\"}";
            var baseUri = new Uri("http://localhost");
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(jsonResponse);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, "api/v1/channels/")))
                .Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Get()).Throws(new HttpException(httpResponse.Object));
            var classUnderTest = new HttpSession(baseUri, TOKEN, USER_ID, httpClient.Object, Mock.Of<IRestService>());

            try
            {
                classUnderTest.FetchChannelList();
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(errorMessage);
                mex.Details.Should().Be(detailedError);
            }
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
            var sut = new HttpSession(baseUri, TOKEN, USER_ID, null, restService.Object);

            var result = sut.GetRootPost(POST_ID);

            Assert.That(result, Is.EqualTo(thread.Posts[ROOT_ID]));
        }

        private static HttpSession SetupUserSessionForFetchingChannelList(Mock<IHttpRequest> httpRequest,
            string jsonResponse)
        {
            var baseUri = new Uri("http://localhost");

            var httpResonse = new Mock<IHttpResponse>();
            httpResonse.Setup(x => x.GetPayload()).Returns(jsonResponse);

            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Get()).Returns(httpResonse.Object);

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, "api/v1/channels/"))).Returns(httpRequest.Object);
            var classUnderTest = new HttpSession(baseUri, TOKEN, USER_ID, httpClient.Object, Mock.Of<IRestService>());
            return classUnderTest;
        }
    }
}