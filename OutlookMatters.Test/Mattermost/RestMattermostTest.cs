using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Http;
using OutlookMatters.Mattermost;
using OutlookMatters.Mattermost.Session;

namespace OutlookMatters.Test.Mattermost
{
    [TestFixture]
    public class RestMattermostTest
    {
        [Test]
        public void LoginByUsername_ReturnsSessionObject()
        {
            const string url = "http://localhost";
            const string token = "token";
            const string userId = "userId";
            const string teamId = "teamId";
            const string username = "username";
            const string password = "password";
            const string jsonPost =
                "{\"name\":\"" + teamId + "\",\"email\":\"" + username + "\",\"password\":\"" + password + "\"}";
            const string jsonResponse =
                "{\"id\":\"" + userId + "\"}";
            var session = new Mock<ISession>();
            var sessionFactory = new Mock<ISessionFactory>();
            sessionFactory.Setup(x => x.CreateSession(new Uri(url), token, userId)).Returns(session.Object);
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(jsonResponse);
            httpResponse.Setup(x => x.GetHeaderValue("Token")).Returns(token);
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithContentType("text/json")).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Post(jsonPost)).Returns(httpResponse.Object);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(new Uri(new Uri(url), "api/v1/users/login")))
                .Returns(httpRequest.Object);
            var classUnderTest = new RestMattermost(sessionFactory.Object, httpClient.Object);

            var result = classUnderTest.LoginByUsername(url, teamId, username, password);

            result.ShouldBeEquivalentTo(session.Object, "because the correct session should be returned");
        }

        [Test]
        public void LoginByUsername_DisposesHttpResponse()
        {
            const string url = "http://localhost";
            const string teamId = "teamId";
            const string username = "username";
            const string password = "password";
            const string jsonResponse = "{\"id\":\"userid\"}";
            var session = new Mock<ISession>();
            var sessionFactory = new Mock<ISessionFactory>();
            sessionFactory.Setup(x => x.CreateSession(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(session.Object);
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(jsonResponse);
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Post(It.IsAny<string>())).Returns(httpResponse.Object);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(It.IsAny<Uri>()))
                .Returns(httpRequest.Object);
            var classUnderTest = new RestMattermost(sessionFactory.Object, httpClient.Object);

            classUnderTest.LoginByUsername(url, teamId, username, password);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void LoginByUsername_ThrowsMattermostException_IfHttpExceptionIsThrown()
        {
            const string url = "http://localhost";
            const string teamId = "teamId";
            const string username = "username";
            const string password = "password";
            const string errorMessage = "error message";
            const string detailedError = "detailed error";
            const string jsonResponse =
                "{\"message\":\"" + errorMessage + "\",\"detailed_error\":\"" + detailedError + "\"}";
            var sessionFactory = new Mock<ISessionFactory>();
            var httpResponse = new Mock<IHttpResponse>();
            httpResponse.Setup(x => x.GetPayload()).Returns(jsonResponse);
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.WithContentType(It.IsAny<string>())).Returns(httpRequest.Object);
            var httpException = new HttpException(httpResponse.Object);
            httpRequest.Setup(x => x.Post(It.IsAny<string>())).Throws(httpException);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Request(It.IsAny<Uri>()))
                .Returns(httpRequest.Object);
            var classUnderTest = new RestMattermost(sessionFactory.Object, httpClient.Object);

            try
            {
                classUnderTest.LoginByUsername(url, teamId, username, password);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(errorMessage);
                mex.Details.Should().Be(detailedError);
            }
        }
    }
}