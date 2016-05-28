using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v3
{
    [TestFixture]
    public class ClientImplTest
    {
        [Test]
        public void LoginByUsername_UsesDataFromRestServiceToCreateSession()
        {
            const string url = "http://localhost";
            const string userId = "userId";
            const string teamId = "teamId";
            const string username = "username";
            const string password = "password";
            var token = "token";
            var login = new Login {LoginId = username, Password = password, Token = string.Empty};
            var user = new User {Id = userId};
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.Login(new Uri(url), login, out token)).Returns(user);
            var session = new Mock<ISession>();
            var chatFactory = new Mock<IChatFactory>();
            chatFactory.Setup(x => x.NewInstance(new Uri(url), token, userId)).Returns(session.Object);
            var sut = new ClientImpl(restService.Object, chatFactory.Object);

            var result = sut.LoginByUsername(url, teamId, username, password);

            result.ShouldBeEquivalentTo(session.Object, "because the correct session should be returned");
        }
    }
}