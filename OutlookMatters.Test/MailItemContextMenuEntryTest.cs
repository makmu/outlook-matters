using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Office.Core;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OutlookMatters.ContextMenu;
using OutlookMatters.Error;
using OutlookMatters.Mail;
using OutlookMatters.Mattermost;
using OutlookMatters.Mattermost.Session;
using OutlookMatters.Security;
using OutlookMatters.Settings;
using OutlookMatters.Test.TestUtils;
using OutlookMatters.Utils;

namespace OutlookMatters.Test
{
    [TestFixture]
    public class MailItemContextMenuEntryTest
    {
        private static ISettingsLoadService DefaultSettingsLoadService
        {
            get
            {
                var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId",
                    "username", "channels");
                var settingsLoadService = new Mock<ISettingsLoadService>();
                settingsLoadService.Setup(x => x.Load()).Returns(settings);
                return settingsLoadService.Object;
            }
        }

        private static IRibbonControl MockOfRibbonControl()
        {
            const string arbitraryChannelId = "123412341234134";
            var mock = new Mock<IRibbonControl>();
            mock.Setup(x => x.Id).Returns(arbitraryChannelId);
            return mock.Object;
        }

        private static IMailExplorer MockOfMailExplorer()
        {
            var mock = new Mock<IMailExplorer>();
            mock.Setup(x => x.QuerySelectedMailData()).Returns(new MailData("sender", "subject", "body"));
            return mock.Object;
        }

        [Test]
        public void GetCustomUI_ReturnsCustomUiForExplorer()
        {
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                Mock.Of<ISettingsLoadService>(),
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                Mock.Of<ISession>(),
                Mock.Of<IStringProvider>());

            var result = classUnderTest.GetCustomUI("Microsoft.Outlook.Explorer");

            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:dynamicMenu[contains(@getContent, ""GetDynamicMenu"")]",
                    "because there should be a dynamic menu which loads its contents using the 'GetDynamicMenu' function");
        }

        [Test]
        public void GetDynamicMenu_ReturnZeroChannelButtons_IfSettingsHasNoChannelsSaved()
        {
            const string subscribedChannelAttribut = "OnPostIntoChannelClick";
            var channels = string.Empty;
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId",
                "username", channels);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            settingsLoadService.Setup(x => x.Load()).Returns(settings);

            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                settingsLoadService.Object,
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                Mock.Of<ISession>(),
                Mock.Of<IStringProvider>());

            var result = classUnderTest.GetDynamicMenu(Mock.Of<IRibbonControl>());

            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .DoNotContainXmlNode(@"//ns:button[contains(@onAction, """ + subscribedChannelAttribut + @""")]",
                    "because there should be one button for each channel");
        }

        [Test]
        public void GetDynamicMenu_ReturnsNoChannelButtons_IfUserIsNotSubscribedToAnyChannel()
        {
            const string channelName = "FunnyChannelName";
            const string channelId = "1234";
            const string notSubscribedChannelType = "User is not subscribed to this channel";
            const string subscribedChannelAttribut = "OnPostIntoChannelClick";
            var channelList = new ChannelList
            {
                Channels =
                    new List<Channel>
                    {
                        new Channel {ChannelName = channelName, ChannelId = channelId, Type = notSubscribedChannelType}
                    }
            };
            var channels = JsonConvert.SerializeObject(channelList);
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId",
                "username", channels);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            settingsLoadService.Setup(x => x.Load()).Returns(settings);

            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                settingsLoadService.Object,
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                Mock.Of<ISession>(),
                Mock.Of<IStringProvider>());

            var result = classUnderTest.GetDynamicMenu(Mock.Of<IRibbonControl>());

            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .DoNotContainXmlNode(@"//ns:button[contains(@onAction, """ + subscribedChannelAttribut + @""")]",
                    "because there should be one button for each channel");
        }

        [Test]
        public void GetDynamicMenu_ReturnsPostButton_ForSubscribedChannel()
        {
            const string channelButtonIdPrefix = "channel_id-";
            const string channelName = "FunnyChannelName";
            const string channelId = "1234";
            const string subscribedChannelType = "O";
            var channelList = new ChannelList
            {
                Channels =
                    new List<Channel>
                    {
                        new Channel {ChannelName = channelName, ChannelId = channelId, Type = subscribedChannelType}
                    }
            };
            var channels = JsonConvert.SerializeObject(channelList);
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId",
                "username", channels);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            settingsLoadService.Setup(x => x.Load()).Returns(settings);

            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                settingsLoadService.Object,
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                Mock.Of<ISession>(),
                Mock.Of<IStringProvider>());

            var result = classUnderTest.GetDynamicMenu(Mock.Of<IRibbonControl>());

            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:button[contains(@label, ""FunnyChannelName"")]",
                    "because there should be one button for each channel");
            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:button[contains(@id, """ + channelButtonIdPrefix + channelId + @""")]",
                    "because the tag of the button should match the channelId");
        }

        [Test]
        public void GetDynamicMenu_ReturnsReplyButton()
        {
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId",
                "username", string.Empty);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                settingsLoadService.Object,
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                Mock.Of<ISession>(),
                Mock.Of<IStringProvider>());

            var result = classUnderTest.GetDynamicMenu(Mock.Of<IRibbonControl>());

            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:button[contains(@label, ""As Reply..."")]",
                    "because there should be a reply button");
            result.Should()
                .WithNamespace("ns", "http://schemas.microsoft.com/office/2009/07/customui")
                .ContainXmlNode(@"//ns:button[contains(@onAction, ""OnReplyClick"")]",
                    "because the reply button should be connected to the 'OnReplyClick'-Method");
        }

        [Test]
        public void GetDynamicMenu_ReturnsSettingsButton()
        {
            var settings = new OutlookMatters.Settings.Settings(string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                settingsLoadService.Object,
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                Mock.Of<ISession>(),
                Mock.Of<IStringProvider>());

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
        public void OnPostIntoChannelClick_CanHandleUserPasswordAbort()
        {
            var control = MockOfRibbonControl();
            var passwordProvider = new Mock<IPasswordProvider>();
            passwordProvider.Setup(x => x.GetPassword(It.IsAny<string>())).Throws<Exception>();
            var sessionCache = new TransientSession(Mock.Of<IMattermost>(),
                DefaultSettingsLoadService,
                passwordProvider.Object);

            var classUnderTest = new MailItemContextMenuEntry(
                MockOfMailExplorer(),
                DefaultSettingsLoadService,
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                sessionCache,
                Mock.Of<IStringProvider>());

            classUnderTest.OnPostIntoChannelClick(control);
        }

        [Test]
        public void OnPostIntoChannelClick_HandlesMattermostExceptionsWhileCreatingPost()
        {
            var control = MockOfRibbonControl();
            var session = new Mock<ISession>();
            session.Setup(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new MattermostException(new OutlookMatters.Mattermost.DataObjects.Error()));
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(
                MockOfMailExplorer(),
                DefaultSettingsLoadService,
                Mock.Of<ISettingsSaveService>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnPostIntoChannelClick(control);

            errorDisplay.Verify(x => x.Display(It.IsAny<MattermostException>()));
        }

        [Test]
        public void OnPostIntoChannelClick_HandlesAnyExceptionsWhileCreatingPost()
        {
            var control = MockOfRibbonControl();
            var session = new Mock<ISession>();
            session.Setup(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<Exception>();
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(
                MockOfMailExplorer(),
                DefaultSettingsLoadService,
                Mock.Of<ISettingsSaveService>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnPostIntoChannelClick(control);

            errorDisplay.Verify(x => x.Display(It.IsAny<Exception>()));
        }

        [Test]
        public void OnPostIntoChannelClick_CreatesPostUsingSession()
        {
            const string channelId = "funny ChannelId";
            const string channelIdWithPrefix = "channel_id-funny ChannelId";
            var control = new Mock<IRibbonControl>();
            control.Setup(x => x.Id).Returns(channelIdWithPrefix);
            var mailData = new MailData("sender", "subject", "message");
            var session = new Mock<ISession>();
            var explorer = new Mock<IMailExplorer>();
            explorer.Setup(x => x.QuerySelectedMailData()).Returns(mailData);
            var classUnderTest = new MailItemContextMenuEntry(
                explorer.Object,
                Mock.Of<ISettingsLoadService>(),
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnPostIntoChannelClick(control.Object);

            session.Verify(
                x =>
                    x.CreatePost(channelId, ":email: From: sender\n:email: Subject: subject\nmessage",
                        string.Empty));
        }

        [Test]
        public void OnReplyClick_CanHandleUserPermalinkAbort()
        {
            var postIdProvider = new Mock<IStringProvider>();
            postIdProvider.Setup(x => x.Get()).Throws<UserAbortException>();
            var session = new Mock<ISession>();
            var mattermost = new Mock<IMattermost>();
            mattermost.Setup(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(session.Object);
            var errorDisplay = new Mock<IErrorDisplay>();

            var classUnderTest = new MailItemContextMenuEntry(
                MockOfMailExplorer(),
                DefaultSettingsLoadService,
                Mock.Of<ISettingsSaveService>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                postIdProvider.Object);

            classUnderTest.OnReplyClick(Mock.Of<IRibbonControl>());

            session.Verify(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            errorDisplay.Verify(x => x.Display(It.IsAny<Exception>()), Times.Never);
        }

        [Test]
        public void OnReplyClick_CreatesPostWithRootIdUsingSession()
        {
            const string rootId = "rootId";
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username",
                "channels");
            var mailData = new MailData("sender", "subject", "message");
            var session = new Mock<ISession>();
            var explorer = new Mock<IMailExplorer>();
            explorer.Setup(x => x.QuerySelectedMailData()).Returns(mailData);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var rootPostIdProvider = new Mock<IStringProvider>();
            rootPostIdProvider.Setup(x => x.Get()).Returns(rootId);
            var classUnderTest = new MailItemContextMenuEntry(
                explorer.Object,
                settingsLoadService.Object,
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                rootPostIdProvider.Object);

            classUnderTest.OnReplyClick(Mock.Of<IRibbonControl>());

            session.Verify(
                x => x.CreatePost(settings.ChannelId, ":email: From: sender\n:email: Subject: subject\nmessage", rootId));
        }

        [Test]
        public void OnReplyClick_HandlesMattermostExceptionsWhileCreatingPost()
        {
            var session = new Mock<ISession>();
            session.Setup(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new MattermostException(new OutlookMatters.Mattermost.DataObjects.Error()));
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(
                MockOfMailExplorer(),
                DefaultSettingsLoadService,
                Mock.Of<ISettingsSaveService>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnReplyClick(Mock.Of<IRibbonControl>());

            errorDisplay.Verify(x => x.Display(It.IsAny<MattermostException>()));
        }

        [Test]
        public void OnReplyClick_HandlesAnyExceptionsWhileCreatingPost()
        {
            var session = new Mock<ISession>();
            session.Setup(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<Exception>();
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(
                MockOfMailExplorer(),
                DefaultSettingsLoadService,
                Mock.Of<ISettingsSaveService>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnReplyClick(Mock.Of<IRibbonControl>());

            errorDisplay.Verify(x => x.Display(It.IsAny<Exception>()));
        }

        [Test]
        public void OnSettingsClick_OpensSettingsUserInterface()
        {
            var settingsUi = new Mock<ISettingsUserInterface>();
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                Mock.Of<ISettingsLoadService>(),
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                settingsUi.Object,
                Mock.Of<ISession>(),
                Mock.Of<IStringProvider>());

            classUnderTest.OnSettingsClick(Mock.Of<IRibbonControl>());

            settingsUi.Verify(x => x.OpenSettings());
        }
    }
}