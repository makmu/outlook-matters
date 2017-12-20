using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.v4;
using OutlookMatters.Core.Mattermost.v4.Interface;
using Test.OutlookMatters.Core.Http;
using Test.OutlookMatters.Core.Mattermost.v4.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v4
{
    [TestFixture]
    class RestServiceTest
    {
        private const string LOGIN_ID = "login";
        private const string PASSWORD = "password";
        private readonly string TOKEN = "token";
        private const string TEAM_ID = "token";
        private const string TEAM_NAME = "token";
        private readonly string CONTENT_TYPE = "application/json";
        public readonly Uri Uri = new Uri("http://localhost");

        [Test]
        public void Login_OutputsToken()
        {
            var login = SetupExampleLogin();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v4/users/login")
                .WithContentType(CONTENT_TYPE)
                .Post(login.SerializeToPayload())
                .WithToken(TOKEN);
            var sut = new RestService(httpClient.Object);

            string token;
            sut.Login(Uri, login, out token);

            Assert.That(token, Is.EqualTo(TOKEN));
        }

        [Test]
        public void Login_DisposesHttpResponse()
        {
            var login = SetupExampleLogin();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v4/users/login")
                .WithContentType(CONTENT_TYPE)
                .Post(login.SerializeToPayload())
                .WithToken(TOKEN);
            var sut = new RestService(httpClient.Object);

            string token;
            sut.Login(Uri, login, out token);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void GetTeams_ReturnsListOfTeams()
        {
            var teamList = SetupExampleTeamList();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v4/users/me/teams")
                .WithToken(TOKEN).Get().Responses(teamList.SerializeToPayload());
            var sut = new RestService(httpClient.Object);

            var teams = sut.GetTeams(Uri, TOKEN);

            teams.ShouldBeEquivalentTo(teamList);
        }

        private static IEnumerable<Team> SetupExampleTeamList()
        {
            return new[]
            {
                new Team {Id = TEAM_ID, Name = TEAM_NAME},
                new Team {Id = TEAM_ID, Name = TEAM_NAME}
            };
        }

        private static Login SetupExampleLogin()
        {
            return new Login
            {
                LoginId = LOGIN_ID,
                Password = PASSWORD
            };
        }
    }
}