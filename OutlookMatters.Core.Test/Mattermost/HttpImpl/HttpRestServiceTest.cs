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
        [Test]
        public void Login_ReturnsUser()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpClient = MockHttpClientRequestWithResponse(
                restApi: "api/v1/users/login",
                request: login.SerializeToPayload(),
                token: "token",
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
            var httpClient = MockHttpClientRequestWithResponse(
                restApi: "api/v1/users/login",
                request: login.SerializeToPayload(),
                token: "token",
                response: user.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            string token;
            sut.Login(SetupExampleUri(), login, out token);

            Assert.That(token, Is.EqualTo("token"));
        }

        [Test]
        public void Login_DisposesHttpResponse()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpResponse = MockHttpResponse(
                token: "token",
                response: user.SerializeToPayload());
            var httpClient = MockHttpClientRequestWithResponse(
                restApi: "api/v1/users/login",
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
            var httpClient = MockHttpClientRequestWithError(
                restApi: "api/v1/users/login",
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

        private static Uri SetupExampleUri()
        {
            return new Uri("http://localhost");
        }

        private static Mock<IHttpResponse> MockHttpResponse(string response, string token)
        {
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(response);
            httpResponse.Setup(x => x.GetHeaderValue("Token")).Returns(token);
            return httpResponse;
        }

        private static Mock<IHttpClient> MockHttpClientRequestWithResponse(string request, string token, string response,
            string restApi)
        {
            var httpResponse = MockHttpResponse(response, token);
            return MockHttpClientRequestWithResponse(request, httpResponse, restApi);
        }

        private static Mock<IHttpClient> MockHttpClientRequestWithResponse(string request, Mock<IHttpResponse> response,
            string restApi)
        {
            Uri baseUri = SetupExampleUri();
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithContentType("text/json")).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Post(request)).Returns(response.Object);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, restApi)))
                .Returns(httpRequest.Object);
            return httpClient;
        }

        private static Mock<IHttpClient> MockHttpClientRequestWithError(string request, string error, string restApi)
        {
            Uri baseUri = SetupExampleUri();
            var httpResponse = MockHttpResponse(error, null);
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithContentType("text/json")).Returns(httpRequest.Object);
            var httpException = new HttpException(httpResponse.Object);
            httpRequest.Setup(x => x.Post(request)).Throws(httpException);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(baseUri, restApi)))
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
            return new User {Id = "userId"};
        }

        private static Login SetupExampleLogin()
        {
            var login = new Login
            {
                Name = "teamId",
                Email = "username",
                Password = "password"
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
    }
}