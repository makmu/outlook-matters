using System.Net;
using FluentAssertions;
using Microsoft.Office.Core;
using Moq;
using NUnit.Framework;
using OutlookMatters.ContextMenu;
using OutlookMatters.Error;
using OutlookMatters.Mail;
using OutlookMatters.Mattermost;
using OutlookMatters.Security;
using OutlookMatters.Settings;

namespace OutlookMatters.Test
{
    [TestFixture]
    public class MailItemContextMenuEntryTest
    {
        [Test]
        public void GetCustomUI_ReturnsCustomUiForExplorer()
        {
            var classUnderTest = new MailItemContextMenuEntry(Mock.Of<IMailExplorer>(), Mock.Of<IMattermost>(), Mock.Of<ISettingsProvider>(), Mock.Of<IPasswordProvider>(), Mock.Of<IErrorDisplay>(), Mock.Of<ISettingsUserInterface>());

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
            settings.Setup(x => x.TeamId).Returns(teamId);
            settings.Setup(x => x.Url).Returns(url);
            settings.Setup(x => x.Username).Returns(username);
            var passwordProvider = new Mock<IPasswordProvider>();
            passwordProvider.Setup(x => x.GetPassword(username)).Returns(password);
            var explorer = new Mock<IMailExplorer>();
            explorer.Setup(x => x.GetSelectedMailBody()).Returns(message);
            var mattermost = new Mock<IMattermost>();
            mattermost.Setup(x => x.LoginByUsername(url, teamId, username, password)).Returns(session.Object);
            mattermost.Setup(
                x => x.LoginByUsername(url, teamId, username, password))
                .Returns(session.Object);
            var classUnderTest = new MailItemContextMenuEntry(
                explorer.Object,
                mattermost.Object,
                settings.Object,
                passwordProvider.Object,
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>());

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());

            session.Verify(x => x.CreatePost(channelId, message));
        }

        [Test]
        public void OnPostClick_CanHandleUserPasswordAbort()
        {
            var passwordProvider = new Mock<IPasswordProvider>();
            passwordProvider.Setup(x => x.GetPassword(It.IsAny<string>())).Throws<System.Exception>();
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                Mock.Of<IMattermost>(),
                Mock.Of<ISettingsProvider>(),
                passwordProvider.Object,
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>());

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());
        }

        [Test]
        public void OnPostClick_CanHandleWebExceptionsWhileLogginIn()
        {
            var mattermost = new Mock<IMattermost>();
            mattermost.Setup(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<WebException>();
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                mattermost.Object,
                Mock.Of<ISettingsProvider>(),
                Mock.Of<IPasswordProvider>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>());

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());

            errorDisplay.Verify( x => x.Display(It.IsAny<WebException>()));
        }

        [Test]
        public void OnPostClick_CanHandleWebExceptionsWhileCreatingPost()
        {
            var session = new Mock<ISession>();
            session.Setup(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>())).Throws<WebException>();
            var mattermost = new Mock<IMattermost>();
            mattermost.Setup(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(session.Object);
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                mattermost.Object,
                Mock.Of<ISettingsProvider>(),
                Mock.Of<IPasswordProvider>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>());

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());

            errorDisplay.Verify( x => x.Display(It.IsAny<WebException>()));
        }

        [Test]
        public void OnSettingsClick_OpensSettingsUserInterface()
        {
            var settingsUi = new Mock<ISettingsUserInterface>();
            var classUnderTest = new MailItemContextMenuEntry(Mock.Of<IMailExplorer>(), Mock.Of<IMattermost>(),
                Mock.Of<ISettingsProvider>(), Mock.Of<IPasswordProvider>(), Mock.Of<IErrorDisplay>(), settingsUi.Object);

            classUnderTest.OnSettingsClick(Mock.Of<IRibbonControl>());

            settingsUi.Verify( x => x.OpenSettings() );
        }
    }
}
