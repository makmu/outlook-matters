using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Http;
using OutlookMatters.Mattermost;
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
            httpRequest.Verify(x => x.Post(jsonPost));
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
            httpRequest.Verify(x => x.Post(jsonPost));
        }

        [Test]
        public void CreatePost_ThrowsMattermostException_IfHttpExceptionIsThrown()
        {
            const string errorMessage = "error message";
            const string detailedError = "detailed error";
            const string jsonResponse =
                "{\"message\":\"" + errorMessage + "\",\"detailed_error\":\"" + detailedError + "\"}";
            var httpRequest = new Mock<IHttpRequest>();
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(jsonResponse);
            HttpException httpException = new HttpException(httpResponse.Object);
            httpRequest.Setup(x => x.Post(It.IsAny<string>())).Throws(httpException);
            var classUnderTest = SetupUserSessionForCreatingPosts(httpRequest);

            try
            {
                classUnderTest.CreatePost(ChannelId, Message, RootId);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(errorMessage);
                mex.Details.Should().Be(detailedError);
            }
        }

        [Test]
        public void CreatePost_DisposesResponse()
        {
            var httpRequest = new Mock<IHttpRequest>();
            var httpResponse = new Mock<IHttpResponse>();
            httpRequest.Setup(x => x.Post(It.IsAny<string>())).Returns(httpResponse.Object);
            var classUnderTest = SetupUserSessionForCreatingPosts(httpRequest);

            classUnderTest.CreatePost(ChannelId, Message);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void GetPostById_GetsPostsRootId()
        {
            const string response =
                "{\"order\":[\"pts7w4o6rignmm5jkwntk6st1a\"],\"posts\":{\"948swb8oxjf1ifc464ddz8h1ph\":{\"id\":\"948swb8oxjf1ifc464ddz8h1ph\",\"create_at\":1458850996664,\"update_at\":1458850996664,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"root_id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"parent_id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"},\"pts7w4o6rignmm5jkwntk6st1a\":{\"id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"create_at\":1458847220284,\"update_at\":1458850996665,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"root_id\":\"\",\"parent_id\":\"\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"}}}";

            const string postId = "948swb8oxjf1ifc464ddz8h1ph";
            var baseUri = new Uri("http://localhost");
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithHeader(It.IsAny<string>(), It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(response);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, "api/v1/posts/" + postId)))
                .Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Get()).Returns(httpResponse.Object);
            var classUnderTest = new UserSession(baseUri, Token, UserId, httpClient.Object);

            var post = classUnderTest.GetPostById(postId);

            post.root_id.Should().Be("pts7w4o6rignmm5jkwntk6st1a");
        }

        [Test]
        public void GetPostById_ThrowsMattermostException_IfHttpExceptionIsThrown()
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
            var classUnderTest = new UserSession(baseUri, Token, UserId, httpClient.Object);

            try
            {
                classUnderTest.GetPostById(postId);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(errorMessage);
                mex.Details.Should().Be(detailedError);
            }
        }

        [Test]
        public void GetPostById_DisposesHttpResponse()
        {
            const string response =
                "{\"order\":[\"pts7w4o6rignmm5jkwntk6st1a\"],\"posts\":{\"948swb8oxjf1ifc464ddz8h1ph\":{\"id\":\"948swb8oxjf1ifc464ddz8h1ph\",\"create_at\":1458850996664,\"update_at\":1458850996664,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"root_id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"parent_id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"},\"pts7w4o6rignmm5jkwntk6st1a\":{\"id\":\"pts7w4o6rignmm5jkwntk6st1a\",\"create_at\":1458847220284,\"update_at\":1458850996665,\"delete_at\":0,\"user_id\":\"izcjneaxrbbr3y13wh9dg94twr\",\"channel_id\":\"6oadmtc9upfwxqy15hg9id6o8o\",\"root_id\":\"\",\"parent_id\":\"\",\"original_id\":\"\",\"message\":\":email: From: \\n:email: Subject: blub\\nFoobar asdf lorem ipsum\",\"type\":\"\",\"props\":{},\"hashtags\":\"\",\"filenames\":[],\"pending_post_id\":\"\"}}}";

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
            var classUnderTest = new UserSession(baseUri, Token, UserId, httpClient.Object);

            classUnderTest.GetPostById(postId);

            httpResponse.Verify(x => x.Dispose());
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