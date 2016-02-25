using FluentAssertions;
using Microsoft.Office.Core;
using Moq;
using NUnit.Framework;

namespace OutlookMatters.Test
{
    [TestFixture]
    public class MailItemContextMenuEntryTest
    {
        [Test]
        public void GetCustomUI_ReturnsCustomUiForExplorer()
        {
            var classUnderTest = new MailItemContextMenuEntry(Mock.Of<IMailExplorer>(), Mock.Of<IMattermost>(), Mock.Of<ISettingsProvider>());

            var result = classUnderTest.GetCustomUI("Microsoft.Outlook.Explorer");

            result.Should().NotBeEmpty("because there should be custom UI xml for the outlook explorer");
        }

        [Test]
        public void OnPostClick_CreatesPostUsingSession()
        {
            const string url = "http://localhost";
            const string teamId = "team";
            const string username = "username";
            const string password = "password";
            const string channelId = "channelId";
            const string message = "message";
            var session = new Mock<ISession>();
            var settings = new Mock<ISettingsProvider>();
            settings.Setup(x => x.ChannelId).Returns(channelId);
            settings.Setup(x => x.Password).Returns(password);
            settings.Setup(x => x.TeamId).Returns(teamId);
            settings.Setup(x => x.Url).Returns(url);
            settings.Setup(x => x.Username).Returns(username);
            var explorer = new Mock<IMailExplorer>();
            explorer.Setup(x => x.GetSelectedMailBody()).Returns(message);
            var mattermost = new Mock<IMattermost>();
            mattermost.Setup(x => x.LoginByUsername(url, teamId, username, password)).Returns(session.Object);
            mattermost.Setup(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Mock.Of<ISession>());
            var classUnderTest = new MailItemContextMenuEntry(explorer.Object, mattermost.Object, settings.Object);

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());

            session.Verify(x => x.CreatePost(channelId, message));
        }
    }
}
