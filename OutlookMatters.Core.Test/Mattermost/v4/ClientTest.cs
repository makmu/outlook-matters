using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v4
{
    [TestFixture]
    public class ClientTest
    {
        [Test]
        public void LoginByUsername_UsesDataFromRestServiceToCreateSession()
        {
            const string url = "http://localhost";
            const string teamId = "teamId";
            const string teamGuid = "teamGuid";
            const string username = "username";
            const string password = "password";
            string token = null;
            var teams = new[] {new Team {Id = teamGuid, Name = teamId}};
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.GetTeams(new Uri(url), token)).Returns(teams);
            var session = new Mock<ISession>();
            var chatFactory = new Mock<IChatFactory>();
            chatFactory.Setup(x => x.NewInstance(restService.Object, new Uri(url), token, teamGuid))
                .Returns(session.Object);
            var sut = new Client(restService.Object, chatFactory.Object);

            var result = sut.LoginByUsername(url, teamId, username, password);

            result.ShouldBeEquivalentTo(session.Object, "because the correct session should be returned");
        }

        [Test]
        public void LoginByUsername_ThrowsChatException_IfSettingsTeamIdDoesNotMatchInitialLoadTeamId()
        {
            const string teamIdThatDoesNotMatchSettingsTeamId = "teamIdThatDoesNotMatch";
            const string settingsTeamId = "settingsTeamId";
            const string url = "http://localhost";
            const string teamGuid = "teamGuid";
            const string username = "username";
            const string password = "password";
            string token = null;
            var teams =  new[] { new Team { Id = teamGuid, Name = teamIdThatDoesNotMatchSettingsTeamId } } ;
            var restService = new Mock<IRestService>();
            restService.Setup(x => x.GetTeams(new Uri(url), token)).Returns(teams);
            var session = new Mock<ISession>();
            var chatFactory = new Mock<IChatFactory>();
            chatFactory.Setup(x => x.NewInstance(restService.Object, new Uri(url), token, teamGuid)).Returns(session.Object);
            var sut = new Client(restService.Object, chatFactory.Object);

            Action action = () => sut.LoginByUsername(url, settingsTeamId, username, password);

            action.ShouldThrow<ChatException>();
        }
    }
}