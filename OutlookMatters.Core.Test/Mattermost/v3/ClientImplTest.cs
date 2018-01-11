using System;
using System.Net;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost;
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
            const string teamId = "teamId";
            const string teamGuid = "teamGuid";
            const string username = "username";
            const string password = "password";
            var token = "token";
            var initialLoad = new InitialLoad {Teams = new[] {new Team {Id = teamGuid, Name = teamId}}};
            var authenticationService = new Mock<IAuthenticationService>();
            authenticationService.Setup(x => x.Login(new Uri(url), username, password, out token));
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.GetInitialLoad(new Uri(url), token)).Returns(initialLoad);
            var session = new Mock<ISession>();
            var chatFactory = new Mock<IChatFactory>();
            chatFactory.Setup(x => x.NewInstance(restService.Object, new Uri(url), token, teamGuid))
                .Returns(session.Object);
            var sut = new ClientImpl(authenticationService.Object, restService.Object, chatFactory.Object);

            var result = sut.LoginByUsername(url, teamId, username, password);

            result.ShouldBeEquivalentTo(session.Object, "because the correct session should be returned");
        }

        [Test]
        public void LoginByUsername_ThrowsChatException_IfSettingsTeamIdDoesNotMatchInitialLoadTeamId()
        {
            const string teamIdThatDoesNotMatchSettingsTeamId = "teamIdThatDoesNotMatch";
            const string SettingsTeamId = "settingsTeamId";
            const string url = "http://localhost";
            const string teamGuid = "teamGuid";
            const string username = "username";
            const string password = "password";
            var token = "token";
            var initialLoad = new InitialLoad { Teams = new[] { new Team { Id = teamGuid, Name = teamIdThatDoesNotMatchSettingsTeamId } } };
            var restService = new Mock<IRestService>();
            var authenticationService = new Mock<IAuthenticationService>();
            authenticationService.Setup(x => x.Login(new Uri(url), username, password, out token));
            restService.Setup(x => x.GetInitialLoad(new Uri(url), token)).Returns(initialLoad);
            var session = new Mock<ISession>();
            var chatFactory = new Mock<IChatFactory>();
            chatFactory.Setup(x => x.NewInstance(restService.Object, new Uri(url), token, teamGuid)).Returns(session.Object);
            var sut = new ClientImpl(authenticationService.Object, restService.Object, chatFactory.Object);

            NUnit.Framework.Constraints.ActualValueDelegate<ISession> performLogin = () => sut.LoginByUsername(url, SettingsTeamId, username, password);

            Assert.That(performLogin, Throws.TypeOf<ChatException>());
        }
    }
}