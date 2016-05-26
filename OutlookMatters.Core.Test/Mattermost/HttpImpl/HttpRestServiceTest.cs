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
    public class HttpRestServiceTest
    {
        const string TOKEN = "token";
        const string CHANNEL_ID = "channelId";
        const string MESSAGE = "message";
        const string TEAM_ID = "teamId";
        const string USER_ID = "userId";
        const string USER_EMAIL = "user@norely.com";
        const string USER_PASSWORD = "secret";

        [Test]
        public void Login_ReturnsUser()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpClient = MockHttpClientUnauthorizedRequestWithResponse(
                restPath: "api/v1/users/login",
                request: login.SerializeToPayload(),
                token: TOKEN,
                response: user.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            string token;
            var result = sut.Login(SetupExampleUri(), login, out token);

            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public void Login_OutsToken()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpClient = MockHttpClientUnauthorizedRequestWithResponse(
                restPath: "api/v1/users/login",
                request: login.SerializeToPayload(),
                token: TOKEN,
                response: user.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            string token;
            sut.Login(SetupExampleUri(), login, out token);

            Assert.That(token, Is.EqualTo(TOKEN));
        }

        [Test]
        public void Login_DisposesHttpResponse()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpResponse = MockHttpResponse(
                token: TOKEN,
                payload: user.SerializeToPayload());
            var httpClient = MockHttpClientUnauthorizedRequestWithResponse(
                restPath: "api/v1/users/login",
                request: login.SerializeToPayload(),
                response: httpResponse);
            var sut = new HttpRestService(httpClient.Object);

            string token;
            sut.Login(SetupExampleUri(), login, out token);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void Login_ThrowsMattermostExceptionWithError_IfMattermostReturnsError()
        {
            var login = SetupExampleLogin();
            var error = SetupExampleError();
            var httpClient = MockHttpClientUnauthorizedRequestWithError(
                restPath: "api/v1/users/login",
                request: login.SerializeToPayload(),
                error: error.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            try
            {
                string token;
                sut.Login(SetupExampleUri(), login, out token);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(error.message);
                mex.Details.Should().Be(error.detailed_error);
            }
        }

        [Test]
        public void CreatePost_MakesTheCorrectHttpRequests()
        {
            var post = SetupExamplePost();
            var httpClient = MockHttpClientRequestWithResponse(
                restPath: "api/v1/channels/" + post.ChannelId + "/create",
                request: post.SerializeToPayload(),
                token: TOKEN,
                response: string.Empty);
            var sut = new HttpRestService(httpClient.Object);

            sut.CreatePost(SetupExampleUri(), TOKEN, CHANNEL_ID, post);

            httpClient.VerifyAll();
        }

        [Test]
        public void CreatePost_DisposesHttpResponse()
        {
            var post = SetupExamplePost();
            var httpResponse = MockHttpResponse(
                token: TOKEN,
                payload: string.Empty);
            var httpClient = MockHttpClientRequestWithResponse(
                restPath: "api/v1/channels/" + post.ChannelId + "/create",
                token: TOKEN,
                request: post.SerializeToPayload(),
                response: httpResponse);
            var sut = new HttpRestService(httpClient.Object);

            sut.CreatePost(SetupExampleUri(), TOKEN, CHANNEL_ID, post);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void CreatePost_ThrowsMattermostExceptionWithError_IfMattermostReturnsError()
        {
            var post = SetupExamplePost();
            var error = SetupExampleError();
            var httpClient = MockHttpClientRequestWithError(
                restPath: "api/v1/channels/" + post.ChannelId + "/create",
                token: TOKEN,
                request: post.SerializeToPayload(),
                error: error.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            try
            {
                sut.CreatePost(SetupExampleUri(), TOKEN, CHANNEL_ID, post);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(error.message);
                mex.Details.Should().Be(error.detailed_error);
            }
        }

        private Post SetupExamplePost()
        {
            return new Post
            {
                Id = string.Empty,
                ChannelId = CHANNEL_ID,
                Message = MESSAGE,
                UserId = USER_ID,
                RootId = string.Empty
            };
        }

        private static Uri SetupExampleUri()
        {
            return new Uri("http://localhost");
        }

        private static Mock<IHttpResponse> MockHttpResponse(string payload, string token)
        {
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(payload);
            httpResponse.Setup(x => x.GetHeaderValue("Token")).Returns(token);
            return httpResponse;
        }

        private static Mock<IHttpClient> MockHttpClientUnauthorizedRequestWithResponse(
            string request,
            string token,
            string response,
            string restPath)
        {
            var httpResponse = MockHttpResponse(response, token);
            return MockHttpClientUnauthorizedRequestWithResponse(request, httpResponse, restPath);
        }

        private static Mock<IHttpClient> MockHttpClientRequestWithResponse(
            string request,
            string token,
            string response,
            string restPath)
        {
            var httpResponse = MockHttpResponse(response, token);
            return MockHttpClientRequestWithResponse(request, token, httpResponse, restPath);
        }

        private static Mock<IHttpRequest> MockHttpRequest(string request, Mock<IHttpResponse> response)
        {
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithContentType("text/json")).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Post(request)).Returns(response.Object);
            return httpRequest;
        }

        private static Mock<IHttpClient> MockHttpClientUnauthorizedRequestWithResponse(
            string request,
            Mock<IHttpResponse> response,
            string restPath)
        {
            Uri baseUri = SetupExampleUri();
            var httpRequest = MockHttpRequest(request, response);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, restPath)))
                .Returns(httpRequest.Object);
            return httpClient;
        }

        private static Mock<IHttpClient> MockHttpClientRequestWithResponse(
            string request,
            string token,
            Mock<IHttpResponse> response,
            string restPath)
        {
            Uri baseUri = SetupExampleUri();
            var httpRequest = MockHttpRequest(request, response);
            httpRequest.Setup(x => x.WithHeader("Authorization", "Bearer " + token)).Returns(httpRequest.Object);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, restPath)))
                .Returns(httpRequest.Object);
            return httpClient;
        }

        private static Mock<IHttpClient> MockHttpClientUnauthorizedRequestWithError(
            string request,
            string error,
            string restPath)
        {
            Uri baseUri = SetupExampleUri();
            var httpResponse = MockHttpResponse(error, null);
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithContentType("text/json")).Returns(httpRequest.Object);
            var httpException = new HttpException(httpResponse.Object);
            httpRequest.Setup(x => x.Post(request)).Throws(httpException);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, restPath)))
                .Returns(httpRequest.Object);
            return httpClient;
        }

        private static Mock<IHttpClient> MockHttpClientRequestWithError(
            string request,
            string token,
            string error,
            string restPath)
        {
            Uri baseUri = SetupExampleUri();
            var httpResponse = MockHttpResponse(error, null);
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithContentType("text/json")).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.WithHeader("Authorization", "Bearer " + token)).Returns(httpRequest.Object);
            var httpException = new HttpException(httpResponse.Object);
            httpRequest.Setup(x => x.Post(request)).Throws(httpException);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, restPath)))
                .Returns(httpRequest.Object);
            return httpClient;
        }

        private Error SetupExampleError()
        {
            return new Error
            {
                message = "message",
                detailed_error = "detailed_error"
            };
        }

        private static User SetupExampleUserData()
        {
            return new User {Id = USER_ID};
        }

        private static Login SetupExampleLogin()
        {
            var login = new Login
            {
                Name = TEAM_ID,
                Email = USER_EMAIL,
                Password = USER_PASSWORD
            };
            return login;
        }
    }

    public static class TestHelper
    {
        public static string SerializeToPayload(this Login login)
        {
            return "{\"name\":\"" + login.Name + "\",\"email\":\"" + login.Email + "\",\"password\":\"" + login.Password +
                   "\"}";
        }

        public static string SerializeToPayload(this User user)
        {
            return "{\"id\":\"" + user.Id + "\"}";
        }

        public static string SerializeToPayload(this Error error)
        {
            return "{\"message\":\"" + error.message + "\",\"detailed_error\":\"" + error.detailed_error + "\"}";
        }

        public static string SerializeToPayload(this Post post)
        {
            return "{\"id\":\"" + post.Id + "\",\"channel_id\":\"" + post.ChannelId + "\",\"message\":\"" + post.Message +
                   "\",\"user_id\":\"" + post.UserId + "\",\"RootId\":\"" + post.RootId + "\"}";
        }
    }
}