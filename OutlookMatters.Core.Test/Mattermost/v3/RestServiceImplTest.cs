using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.v3.Interface;
using OutlookMatters.Core.Mattermost.v3;
using Test.OutlookMatters.Core.Http;
using Test.OutlookMatters.Core.Mattermost.v3.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v3
{
    [TestFixture]
    public class RestServiceImplTest
    {
        public const string TOKEN = "token";
        public const string LOGIN_ID = "login";
        public const string PASSWORD = "password";
        public const string USER_ID = "user";
        public readonly Uri Uri = new Uri("http://localhost");

        [Test]
        public void Login_ReturnsUser()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v3/users/login")
                .Post(login.SerializeToPayload())
                .Responses(user.SerializeToPayload())
                .WithToken(TOKEN);
            var sut = new RestServiceImpl(httpClient.Object);

            string token;
            var result = sut.Login(Uri, login, out token);

            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public void Login_OutputsToken()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v3/users/login")
                .Post(login.SerializeToPayload())
                .Responses(user.SerializeToPayload())
                .WithToken(TOKEN);
            var sut = new RestServiceImpl(httpClient.Object);

            string token;
            sut.Login(Uri, login, out token);

            Assert.That(token, Is.EqualTo(TOKEN));
        }

        [Test]
        public void Login_DisposesHttpResponse()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v3/users/login")
                .Post(login.SerializeToPayload())
                .Responses(user.SerializeToPayload())
                .WithToken(TOKEN);
            var sut = new RestServiceImpl(httpClient.Object);

            string token;
            sut.Login(Uri, login, out token);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void Login_ThrowsMattermostExceptionWithError_IfHttpExceptionWithErrorPayload()
        {
            var login = SetupExampleLogin();
            var error = SetupExampleError();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v3/users/login")
                .FailsAtPost(login.SerializeToPayload())
                .Responses(error.SerializeToPayload());
            var sut = new RestServiceImpl(httpClient.Object);

            try
            {
                string token;
                sut.Login(Uri, login, out token);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(error.Message);
                mex.Details.Should().Be(error.DetailedError);
            }
        }

        private static Login SetupExampleLogin()
        {
            return new Login
            {
                LoginId = LOGIN_ID,
                Password = PASSWORD,
                Token = string.Empty
            };
        }

        private static User SetupExampleUserData()
        {
            return new User {Id = USER_ID};
        }

        private Error SetupExampleError()
        {
            return new Error
            {
                Message = "message",
                DetailedError = "detailed_error"
            };
        }
    }
}
