using System;
using System.CodeDom;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Http;
using OutlookMatters.Mattermost;

namespace OutlookMatters.Test
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
            httpRequest.Setup(x => x.SendRequest(jsonPost)).Returns(httpResponse.Object);
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.Post(new Uri(new Uri(url), "api/v1/users/login")))
                .Returns(httpRequest.Object);
            var classUnderTest = new RestMattermost(sessionFactory.Object, httpClient.Object);

            var result = classUnderTest.LoginByUsername(url, teamId, username, password);

            result.ShouldBeEquivalentTo(session.Object, "because the correct session should be returned");
        }
    }
}
