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
        private const string TEAM_ID = "team id";
        private const string TEAM_NAME = "team name";
        private const string CHANNEL_ID = "channel id";
        private const string CHANNEL_NAME = "channel name";
        private const string POST_ID = "post id";
        private const string MESSAGE = "message";
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
        public void GetTeams_PerformsRestCall()
        {
            var teamList = SetupExampleTeamList();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v4/users/me/teams")
                .WithToken(TOKEN).Get().Responses(teamList.SerializeToPayload());
            var sut = new RestService(httpClient.Object);

            var teams = sut.GetTeams(Uri, TOKEN);

            teams.ShouldBeEquivalentTo(teamList);
        }

        [Test]
        public void GetTeams_DisposesHttpResponse()
        {
            var teamList = SetupExampleTeamList();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v4/users/me/teams")
                .WithToken(TOKEN).Get().Responses(teamList.SerializeToPayload());
            var sut = new RestService(httpClient.Object);

            sut.GetTeams(Uri, TOKEN);

            httpResponse.Verify(h => h.Dispose());
        }

        [Test]
        public void GetChannelList_PerformsRestCall()
        {
            var channelList = SetupExampleChannelList();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v4/users/me/teams/" + TEAM_ID + "/channels")
                .WithToken(TOKEN)
                .Get()
                .Responses(channelList.SerializeToPayload());
            var sut = new RestService(httpClient.Object);

            var channels = sut.GetChannels(Uri, TOKEN, TEAM_ID);

            channels.ShouldBeEquivalentTo(channelList);
        }

        [Test]
        public void GetChannelList_DisposesHttpResponse()
        {
            var channelList = SetupExampleChannelList();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v4/users/me/teams/" + TEAM_ID + "/channels")
                .WithToken(TOKEN)
                .Get()
                .Responses(channelList.SerializeToPayload());
            var sut = new RestService(httpClient.Object);

            sut.GetChannels(Uri, TOKEN, TEAM_ID);

            httpResponse.Verify(h => h.Dispose());
        }

        [Test]
        public void GetPostById_GetsPostViaHttp()
        {
            var post = SetupExamplePost();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v4/posts/" + POST_ID)
                .WithContentType(CONTENT_TYPE)
                .WithToken(TOKEN)
                .Get()
                .Responses(post.SerializeToPayload());
            var sut = new RestService(httpClient.Object);

            var result = sut.GetPostById(Uri, TOKEN, POST_ID);

            result.ShouldBeEquivalentTo(post);
        }

        [Test]
        public void GetPostById_DisposesHttpResonse()
        {
            var post = SetupExamplePost();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse =
                httpClient.SetupRequest("http://localhost/", "api/v4/posts/" + POST_ID)
                .WithContentType(CONTENT_TYPE)
                    .WithToken(TOKEN)
                    .Get().Responses(post.SerializeToPayload());
            var sut = new RestService(httpClient.Object);

            sut.GetPostById(Uri, TOKEN, POST_ID);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void CreatePost_MakesTheCorrectHttpRequests()
        {
            var post = SetupExamplePost();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v4/posts")
                .WithContentType(CONTENT_TYPE)
                .WithToken(TOKEN)
                .Post(post.SerializeToPayload());
            var sut = new RestService(httpClient.Object);

            sut.CreatePost(Uri, TOKEN, post);

            httpClient.VerifyAll();
        }

        private static IEnumerable<Team> SetupExampleTeamList()
        {
            return new[]
            {
                new Team {Id = TEAM_ID, Name = TEAM_NAME},
                new Team {Id = TEAM_ID, Name = TEAM_NAME}
            };
        }

        private static IEnumerable<Channel> SetupExampleChannelList()
        {
            return new[]
            {
                new Channel {Id = CHANNEL_ID, Name = CHANNEL_NAME},
                new Channel {Id = CHANNEL_ID, Name = CHANNEL_NAME}
            };
        }

        private static Post SetupExamplePost()
        {
            return new Post
            {
                Id = string.Empty,
                ChannelId = CHANNEL_ID,
                Message = MESSAGE
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