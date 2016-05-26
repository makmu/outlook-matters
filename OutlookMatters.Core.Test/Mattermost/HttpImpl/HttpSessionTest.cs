using System;
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
        public void GetRootPost_ThrowsMattermostException_IfHttpExceptionIsThrown()
        {
            const string errorMessage = "error message";
            const string detailedError = "detailed error";
            const string jsonResponse =
                "{\"message\":\"" + errorMessage + "\",\"detailed_error\":\"" + detailedError + "\"}";
            const string postId = "948swb8oxjf1ifc464ddz8h1ph";
            var baseUri = new Uri("http://localhost");
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(jsonResponse);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, "api/v1/posts/" + postId)))
                .Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Get()).Throws(new HttpException(httpResponse.Object));
            var classUnderTest = new HttpSession(baseUri, TOKEN, USER_ID, httpClient.Object, Mock.Of<IRestService>());

            try
            {
                classUnderTest.GetRootPost(postId);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(errorMessage);
                mex.Details.Should().Be(detailedError);
            }
        }

        [Test]
        public void GetRootPost_DisposesHttpResponse()
        {
            const string response =
                "{\"order\":[\"pts7w4o6rignmm5jkwntk6st1a\"],\"posts\":{\"948swb8oxjf1ifc464ddz8h1ph\":{\"id\":\"948swb8oxjf1ifc464ddz8h1ph\",\"create_at\":1458850996664,\"update_at\":1458850996664,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"RootId\":\"pts7w4o6rignmm5jkwntk6st1a\",\"parent_id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"},\"pts7w4o6rignmm5jkwntk6st1a\":{\"id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"create_at\":1458847220284,\"update_at\":1458850996665,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"RootId\":\"\",\"parent_id\":\"\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"}}}";

            const string postId = "948swb8oxjf1ifc464ddz8h1ph";

            var baseUri = new Uri("http://localhost");
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(response);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(It.IsAny<Uri>()))
                .Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Get()).Returns(httpResponse.Object);
            var classUnderTest = new HttpSession(baseUri, TOKEN, USER_ID, httpClient.Object, Mock.Of<IRestService>());

            classUnderTest.GetRootPost(postId);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void GetRootPost_ReturnsRootPost_IfParentIsNotRootPost()
        {
            const string response =
                "{\"order\":[\"pts7w4o6rignmm5jkwntk6st1a\"],\"posts\":{\"948swb8oxjf1ifc464ddz8h1ph\":{\"id\":\"948swb8oxjf1ifc464ddz8h1ph\",\"create_at\":1458850996664,\"update_at\":1458850996664,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"RootId\":\"pts7w4o6rignmm5jkwntk6st1a\",\"parent_id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"},\"pts7w4o6rignmm5jkwntk6st1a\":{\"id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"create_at\":1458847220284,\"update_at\":1458850996665,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"RootId\":\"\",\"parent_id\":\"\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"}}}";

            const string postId = "948swb8oxjf1ifc464ddz8h1ph";
            const string rootId = "pts7w4o6rignmm5jkwntk6st1a";
            var baseUri = new Uri("http://localhost");
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(response);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(It.IsAny<Uri>()))
                .Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Get()).Returns(httpResponse.Object);
            var classUnderTest = new HttpSession(baseUri, TOKEN, USER_ID, httpClient.Object, Mock.Of<IRestService>());

            var result = classUnderTest.GetRootPost(postId);

            Assert.That(result.Id, Is.EqualTo(rootId));
        }

        [Test]
        public void GetRootPost_ReturnsRootPost_IfRootIdOfPostIsEmpty()
        {
            const string response =
                "{\"order\":[\"pts7w4o6rignmm5jkwntk6st1a\"],\"posts\":{\"948swb8oxjf1ifc464ddz8h1ph\":{\"id\":\"948swb8oxjf1ifc464ddz8h1ph\",\"create_at\":1458850996664,\"update_at\":1458850996664,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"RootId\":\"pts7w4o6rignmm5jkwntk6st1a\",\"parent_id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"},\"pts7w4o6rignmm5jkwntk6st1a\":{\"id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"create_at\":1458847220284,\"update_at\":1458850996665,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"RootId\":\"\",\"parent_id\":\"\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"}}}";

            const string postId = "pts7w4o6rignmm5jkwntk6st1a";
            var baseUri = new Uri("http://localhost");
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(response);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(It.IsAny<Uri>()))
                .Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Get()).Returns(httpResponse.Object);
            var classUnderTest = new HttpSession(baseUri, TOKEN, USER_ID, httpClient.Object, Mock.Of<IRestService>());

            var result = classUnderTest.GetRootPost(postId);

            Assert.That(result.Id, Is.EqualTo(postId));
        }

        private static HttpSession SetupUserSessionForCreatingPosts(Mock<IHttpRequest> httpRequest)
        {
            var baseUri = new Uri("http://localhost");
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, "api/v1/channels/" + CHANNEL_ID + "/create")))
                .Returns(httpRequest.Object);
            var classUnderTest = new HttpSession(baseUri, TOKEN, USER_ID, httpClient.Object, Mock.Of<IRestService>());
            return classUnderTest;
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