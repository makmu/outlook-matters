using FluentAssertions;
using Microsoft.Office.Core;
using Moq;
using NUnit.Framework;
using OutlookMatters.ContextMenu;
using OutlookMatters.Error;
using OutlookMatters.Mail;
using OutlookMatters.Mattermost;
using OutlookMatters.Mattermost.Session;
using OutlookMatters.Security;
using OutlookMatters.Settings;
using OutlookMatters.Test.TestUtils;
using System.Net;

namespace OutlookMatters.Test
{
    [TestFixture]
    public class MailItemContextMenuEntryTest
    {
        [Test]
        public void GetCustomUI_ReturnsCustomUiForExplorer()
        {
            var classUnderTest = new MailItemContextMenuEntry(Mock.Of<IMailExplorer>(), Mock.Of<ISettingsLoadService>(), Mock.Of<IErrorDisplay>(), Mock.Of<ISettingsUserInterface>(), Mock.Of<ISessionCache>());

            var result = classUnderTest.GetCustomUI("Microsoft.Outlook.Explorer");

            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:dynamicMenu[contains(@getContent, ""GetDynamicMenu"")]", 
                "because there should be a dynamic menu which loads its contents using the 'GetDynamicMenu' function");
        }

        [Test]
        public void GetDynamicMenu_ReturnsSettingsButton()
        {
            var classUnderTest = new MailItemContextMenuEntry(Mock.Of<IMailExplorer>(), Mock.Of<ISettingsLoadService>(), Mock.Of<IErrorDisplay>(), Mock.Of<ISettingsUserInterface>(), Mock.Of<ISessionCache>());

            var result = classUnderTest.GetDynamicMenu(Mock.Of<IRibbonControl>());

            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:button[contains(@label, ""Settings..."")]",
                    "because there should always be a settings button");
            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:button[contains(@onAction, ""OnSettingsClick"")]",
                    "because the settings button should be connected to the 'OnSettingsClick'-Method");
        }

        [Test]
        public void GetDynamicMenu_ReturnsPostButton()
        {
            var classUnderTest = new MailItemContextMenuEntry(Mock.Of<IMailExplorer>(), Mock.Of<ISettingsLoadService>(), Mock.Of<IErrorDisplay>(), Mock.Of<ISettingsUserInterface>(), Mock.Of<ISessionCache>());

            var result = classUnderTest.GetDynamicMenu(Mock.Of<IRibbonControl>());

            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:button[contains(@label, ""Post"")]",
                    "because there should always be a post button");
            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:button[contains(@onAction, ""OnPostClick"")]",
                    "because the post button should be connected to the 'OnPostClick'-Method");
        }

        [Test]
        public void OnPostClick_CreatesPostUsingSession()
        {
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username");
            const string password = "password";
            var mailData = new MailData("sender", "subject", "message");
            var session = new Mock<ISession>();
            var sessionCache = new Mock<ISessionCache>();
            var explorer = new Mock<IMailExplorer>();
            explorer.Setup(x => x.QuerySelectedMailData()).Returns(mailData);
            sessionCache.SetupGet(x => x.Session).Returns(session.Object);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var classUnderTest = new MailItemContextMenuEntry(explorer.Object, settingsLoadService.Object, Mock.Of<IErrorDisplay>(), Mock.Of<ISettingsUserInterface>(), sessionCache.Object);

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());

            session.Verify(x => x.CreatePost(settings.ChannelId, ":email: From: sender\n:email: Subject: subject\nmessage"));
        }

        [Test]
        public void OnPostClick_CanHandleUserPasswordAbort()
        {
            var passwordProvider = new Mock<IPasswordProvider>();
            passwordProvider.Setup(x => x.GetPassword(It.IsAny<string>())).Throws<System.Exception>();
            var sessionCache = new UserSessionCache(Mock.Of<IMattermost>(),
                DefaultSettingsLoadService,
                passwordProvider.Object);

            var classUnderTest = new MailItemContextMenuEntry(MockOfMailExplorer(), DefaultSettingsLoadService, Mock.Of<IErrorDisplay>(), Mock.Of<ISettingsUserInterface>(), sessionCache);

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());
        }

        [Test]
        public void OnPostClick_CanHandleWebExceptionsWhileLogginIn()
        {
            var mattermost = new Mock<IMattermost>();
            mattermost.Setup(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<WebException>();
            var sessionCache = new UserSessionCache(mattermost.Object,
                DefaultSettingsLoadService,
                Mock.Of<IPasswordProvider>());
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(MockOfMailExplorer(), DefaultSettingsLoadService, errorDisplay.Object, Mock.Of<ISettingsUserInterface>(), sessionCache);

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());

            errorDisplay.Verify( x => x.Display(It.IsAny<WebException>()));
        }

        [Test]
        public void OnPostClick_CanHandleWebExceptionsWhileCreatingPost()
        {
            var session = new Mock<ISession>();
            session.Setup(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>())).Throws<WebException>();
            var sessionCache = new Mock<ISessionCache>();
            sessionCache.SetupGet(x => x.Session).Returns(session.Object);
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(MockOfMailExplorer(), DefaultSettingsLoadService, errorDisplay.Object, Mock.Of<ISettingsUserInterface>(), sessionCache.Object);

            classUnderTest.OnPostClick(Mock.Of<IRibbonControl>());

            errorDisplay.Verify( x => x.Display(It.IsAny<WebException>()));
        }

        [Test]
        public void OnSettingsClick_OpensSettingsUserInterface()
        {
            var settingsUi = new Mock<ISettingsUserInterface>();
            var classUnderTest = new MailItemContextMenuEntry(Mock.Of<IMailExplorer>(), Mock.Of<ISettingsLoadService>(), Mock.Of<IErrorDisplay>(), settingsUi.Object, Mock.Of<ISessionCache>());

            classUnderTest.OnSettingsClick(Mock.Of<IRibbonControl>());

            settingsUi.Verify( x => x.OpenSettings() );
        }

        private static ISettingsLoadService DefaultSettingsLoadService
        {
            get
            {
                var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username");
                var settingsLoadService = new Mock<ISettingsLoadService>();
                settingsLoadService.Setup(x => x.Load()).Returns(settings);
                return settingsLoadService.Object;
            }
        }

        private static IMailExplorer MockOfMailExplorer()
        {
            var mock = new Mock<IMailExplorer>();
            mock.Setup(x => x.QuerySelectedMailData()).Returns(new MailData("sender", "subject", "body"));
            return mock.Object;
        }
    }
}
