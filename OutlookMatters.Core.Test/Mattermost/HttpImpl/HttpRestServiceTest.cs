using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.HttpImpl;
using OutlookMatters.Core.Mattermost.Interface;

namespace Test.OutlookMatters.Core.Mattermost.HttpImpl
{
    [TestFixture]
    public class HttpRestServiceTest
    {
        const string TOKEN = "token";
        const string MESSAGE = "message";
        const string TEAM_ID = "teamId";
        const string USER_ID = "userId";
        const string USER_EMAIL = "user@norely.com";
        const string USER_PASSWORD = "secret";
        const string POST_ID = "postId";
        const string CHANNEL_ID = "channelId";
        const string CHANNEL_NAME = "FunnyChannelName";
        const ChannelType CHANNEL_TYPE = ChannelType.Public;

        [Test]
        public void Login_ReturnsUser()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/users/login")
                .Post(login.SerializeToPayload())
                .Responses(user.SerializeToPayload())
                .WithToken(TOKEN);
            var sut = new HttpRestService(httpClient.Object);

            string token;
            var result = sut.Login(SetupExampleUri(), login, out token);

            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public void Login_OutputsToken()
        {
            var login = SetupExampleLogin();
            var user = SetupExampleUserData();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/users/login")
                .Post(login.SerializeToPayload())
                .Responses(user.SerializeToPayload())
                .WithToken(TOKEN);
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
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v1/users/login")
                .Post(login.SerializeToPayload())
                .Responses(user.SerializeToPayload())
                .WithToken(TOKEN);
            var sut = new HttpRestService(httpClient.Object);

            string token;
            sut.Login(SetupExampleUri(), login, out token);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void Login_ThrowsMattermostExceptionWithError_IfHttpExceptionWithErrorPayload()
        {
            var login = SetupExampleLogin();
            var error = SetupExampleError();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/users/login")
                .FailsAtPost(login.SerializeToPayload())
                .Responses(error.SerializeToPayload());
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
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/channels/" + post.ChannelId + "/create")
                .WithToken(TOKEN)
                .Post(post.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            sut.CreatePost(SetupExampleUri(), TOKEN, CHANNEL_ID, post);

            httpClient.VerifyAll();
        }

        [Test]
        public void CreatePost_DisposesHttpResponse()
        {
            var post = SetupExamplePost();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/",
                "api/v1/channels/" + post.ChannelId + "/create")
                .WithToken(TOKEN)
                .Post(post.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            sut.CreatePost(SetupExampleUri(), TOKEN, CHANNEL_ID, post);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void CreatePost_ThrowsMattermostExceptionWithError_IfHttpExceptionWithErrorPayload()
        {
            var post = SetupExamplePost();
            var error = SetupExampleError();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/channels/" + post.ChannelId + "/create")
                .WithToken(TOKEN)
                .FailsAtPost(post.SerializeToPayload())
                .Responses(error.SerializeToPayload());
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

        [Test]
        public void GetThreadOfPosts_GetsThreadViaHttp()
        {
            var thread = SetupExampleThread();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/posts/" + POST_ID)
                .WithToken(TOKEN)
                .Get()
                .Responses(thread.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            var result = sut.GetThreadOfPosts(SetupExampleUri(), TOKEN, POST_ID);

            Assert.That(result, Is.EqualTo(thread));
        }

        [Test]
        public void GetThreadOfPosts_DisposesHttpResonse()
        {
            var thread = SetupExampleThread();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v1/posts/" + POST_ID)
                .WithToken(TOKEN)
                .Get();
            httpResponse.Responses(thread.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            sut.GetThreadOfPosts(SetupExampleUri(), TOKEN, POST_ID);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void GetThreadOfPosts_ThrowsMattermostExceptionWithError_IfHttpExceptionWithErrorPayload()
        {
            var error = SetupExampleError();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/posts/" + POST_ID)
                .WithToken(TOKEN)
                .FailsAtGet()
                .Responses(error.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            try
            {
                sut.GetThreadOfPosts(SetupExampleUri(), TOKEN, POST_ID);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(error.message);
                mex.Details.Should().Be(error.detailed_error);
            }
        }

        [Test]
        public void GetChannelList_GetsThreadViaHttp()
        {
            var channelList = SetupExampleChannelList();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/channels/")
                .WithToken(TOKEN)
                .Get()
                .Responses(channelList.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            var result = sut.GetChannelList(SetupExampleUri(), TOKEN);

            Assert.That(result, Is.EqualTo(channelList));
        }

        [Test]
        public void GetChannelList_DisposesHttpRequest()
        {
            var channelList = SetupExampleChannelList();
            var httpClient = new Mock<IHttpClient>();
            var httpResponse = httpClient.SetupRequest("http://localhost/", "api/v1/channels/")
                .WithToken(TOKEN)
                .Get()
                .Responses(channelList.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            sut.GetChannelList(SetupExampleUri(), TOKEN);

            httpResponse.Verify(x => x.Dispose());
        }

        [Test]
        public void GetChannelList_ThrowsMattermostExceptionWithError_IfHttpExceptionWithErrorPayload()
        {
            var error = SetupExampleError();
            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupRequest("http://localhost/", "api/v1/channels/")
                .WithToken(TOKEN)
                .FailsAtGet()
                .Responses(error.SerializeToPayload());
            var sut = new HttpRestService(httpClient.Object);

            try
            {
                sut.GetChannelList(SetupExampleUri(), TOKEN);
                Assert.Fail();
            }
            catch (MattermostException mex)
            {
                mex.Message.Should().Be(error.message);
                mex.Details.Should().Be(error.detailed_error);
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

        private Thread SetupExampleThread()
        {
            var thread = new Thread
            {
                Order = new[] {POST_ID},
                Posts = new Dictionary<string, Post>()
            };
            thread.Posts[POST_ID] = SetupExamplePost();
            return thread;
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
                   "\",\"user_id\":\"" + post.UserId + "\",\"root_id\":\"" + post.RootId + "\"}";
        }

        public static string SerializeToPayload(this Channel channel)
        {
            return "{\"id\":\"" + channel.ChannelId +
                   "\",\"create_at\":1458911668852,\"update_at\":1458911668852,\"delete_at\":0,\"type\":\"" +
                   channel.Type +
                   "\",\"display_name\":\"" + channel.ChannelName + "\"}";
        }

        public static string SerializeToPayload(this Thread post)
        {
            return
                "{\"order\":[\"" + string.Join(",", post.Order) + "\"],\"posts\":{" +
                string.Join(",", post.Posts.Select(x => "\"" + x.Key + "\":" + x.Value.SerializeToPayload())) +
                "}}";
        }

        public static string SerializeToPayload(this ChannelList channelList)
        {
            return
                "{\"channels\":[" + string.Join(",", channelList.Channels.Select(x => x.SerializeToPayload())) +
                "]}";
        }
    }
}