using System;
using System.Collections.Generic;
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
        const string MESSAGE = "message";
        const string TEAM_ID = "teamId";
        const string TEAM_NAME = "teamName";
        const string USER_EMAIL = "user@norely.com";
        const string USER_PASSWORD = "secret";
        const string POST_ID = "postId";
        const string CHANNEL_ID = "channelId";
        const string CHANNEL_NAME = "FunnyChannelName";
        const ChannelType CHANNEL_TYPE = ChannelType.Public;

        [Test]
        public void Login_PerformsRestCall()
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

        [Test]
        public void GetChannelList_PerformsRestCall()
        {
            var channelList = SetupExampleChannelList();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v3/teams/" + TEAM_ID + "/channels/")
                .WithToken(TOKEN)
                .Get()
                .Responses(channelList.SerializeToPayload());
            var sut = new RestServiceImpl(httpClient.Object);

            var result = sut.GetChannelList(Uri, TOKEN, TEAM_ID);

            Assert.That(result, Is.EqualTo(channelList));
        }

        [Test]
        public void GetChannelList_DisposesHttpRequest()
        {
            var channelList = SetupExampleChannelList();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v3/teams/" + TEAM_ID + "/channels/")
                .WithToken(TOKEN)
                .Get()
                .Responses(channelList.SerializeToPayload());
            var sut = new RestServiceImpl(httpClient.Object);

            sut.GetChannelList(Uri, TOKEN, TEAM_ID);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void GetChannelList_ThrowsMattermostExceptionWithError_IfHttpExceptionWithErrorPayload()
        {
            var error = SetupExampleError();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v3/teams/" + TEAM_ID + "/channels/")
                .WithToken(TOKEN)
                .FailsAtGet()
                .Responses(error.SerializeToPayload());
            var sut = new RestServiceImpl(httpClient.Object);

            try
            {
                sut.GetChannelList(Uri, TOKEN, TEAM_ID);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(error.Message);
                mex.Details.Should().Be(error.DetailedError);
            }
        }

        [Test]
        public void GetInitialLoad_PerformsRestCall()
        {
            var initialLoad = SetupExampleInitialLoad();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v3/users/initial_load")
                .WithToken(TOKEN)
                .Get()
                .Responses(initialLoad.SerializeToPayload());
            var sut = new RestServiceImpl(httpClient.Object);

            var result = sut.GetInitialLoad(Uri, TOKEN);

            Assert.That(result, Is.EqualTo(initialLoad));
        }

        [Test]
        public void GetInitialLoad_DisposesHttpRequest()
        {
            var channelList = SetupExampleChannelList();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v3/users/initial_load")
                .WithToken(TOKEN)
                .Get()
                .Responses(channelList.SerializeToPayload());
            var sut = new RestServiceImpl(httpClient.Object);

            sut.GetInitialLoad(Uri, TOKEN);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void GetInitialLoad_ThrowsMattermostExceptionWithError_IfHttpExceptionWithErrorPayload()
        {
            var error = SetupExampleError();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v3/users/initial_load")
                .WithToken(TOKEN)
                .FailsAtGet()
                .Responses(error.SerializeToPayload());
            var sut = new RestServiceImpl(httpClient.Object);

            try
            {
                sut.GetInitialLoad(Uri, TOKEN);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(error.Message);
                mex.Details.Should().Be(error.DetailedError);
            }
        }

        private static ChannelList SetupExampleChannelList()
        {
            return new ChannelList
            {
                Channels = new List<Channel>
                {
                    new Channel {ChannelId = CHANNEL_ID, ChannelName = CHANNEL_NAME, Type = CHANNEL_TYPE}
                }
            };
        }

        private static InitialLoad SetupExampleInitialLoad()
        {
            return new InitialLoad
            {
                Teams = new[]
                {
                    new Team {Id = TEAM_ID, Name = TEAM_NAME}
                }
            };
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