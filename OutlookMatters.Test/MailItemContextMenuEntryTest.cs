using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OutlookMatters.ContextMenu;
using OutlookMatters.Error;
using OutlookMatters.Mail;
using OutlookMatters.Mattermost;
using OutlookMatters.Mattermost.DataObjects;
using OutlookMatters.Mattermost.Session;
using OutlookMatters.Security;
using OutlookMatters.Settings;
using OutlookMatters.Test.TestUtils;
using OutlookMatters.Utils;
using Exception = System.Exception;

namespace OutlookMatters.Test
{
    [TestFixture]
    public class MailItemContextMenuEntryTest
    {
        private static ISettingsLoadService DefaultSettingsLoadService
        {
            get
            {
                var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId",
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
            mock.Setup(x => x.QuerySelectedMailItem()).Returns(MockMailItem());
            return mock.Object;
        }

        private static MailItem MockMailItem(string sender = "sender", string subject = "subject",
            string body = "message")
        {
            var mock = new Mock<MailItem>();
            mock.Setup(m => m.SenderName).Returns(sender);
            mock.Setup(m => m.Subject).Returns(subject);
            mock.Setup(m => m.Body).Returns(body);

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
        public void GetCustomUI_ReturnsNull_IfRibbonIdIvalid()
        {
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                Mock.Of<ISettingsLoadService>(),
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                Mock.Of<ISession>(),
                Mock.Of<IStringProvider>());

            var result = classUnderTest.GetCustomUI("unknown");

            result.Should().BeNull();
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
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", 
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
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", 
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
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", 
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
            var settings = new OutlookMatters.Settings.Settings(string.Empty, string.Empty,
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
        public void GetDynamicMenu_ReturnZeroChannelButtons_IfSettingsHasNoChannelsSaved()
        {
            const string subscribedChannelAttribut = "OnPostIntoChannelClick";
            var channels = string.Empty;
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId",
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
        public void OnPostIntoChannelClick_CreatesPostUsingSession()
        {
            const string channelId = "funny ChannelId";
            const string channelIdWithPrefix = "channel_id-funny ChannelId";
            var control = new Mock<IRibbonControl>();
            control.Setup(x => x.Id).Returns(channelIdWithPrefix);
            var session = new Mock<ISession>();
            var explorer = new Mock<IMailExplorer>();
            explorer.Setup(x => x.QuerySelectedMailItem()).Returns(MockMailItem());
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
        public void OnRefreshChannelListClick_SavesChannelList()
        {
            const string channelId = "channel id";
            const string channelName = "channel name";
            const string channelType = "channel type";
            var channelList = new ChannelList
            {
                Channels =
                    new List<Channel>
                    {
                        new Channel {ChannelId = channelId, ChannelName = channelName, Type = channelType}
                    }
            };
            var session = new Mock<ISession>();
            session.Setup(x => x.FetchChannelList()).Returns(channelList);
            var settings = new OutlookMatters.Settings.Settings(string.Empty, string.Empty, string.Empty,
                "{\"channels\":[{\"id\":\"channel id\",\"display_name\":\"channel name\",\"type\":\"channel type\"}]}");
            var loadService = new Mock<ISettingsLoadService>();
            loadService.Setup(x => x.Load()).Returns(settings);
            var saveService = new Mock<ISettingsSaveService>();
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                loadService.Object,
                saveService.Object,
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnRefreshChannelListClick(Mock.Of<IRibbonControl>());

            saveService.Verify(x => x.Save(settings));
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
            const string channelId = "channelId";
            var post = new Post {root_id = rootId, channel_id = channelId};
            var session = new Mock<ISession>();
            session.Setup(x => x.GetPostById(rootId)).Returns(post);
            var explorer = new Mock<IMailExplorer>();
            explorer.Setup(x => x.QuerySelectedMailItem()).Returns(MockMailItem());
            var rootPostIdProvider = new Mock<IStringProvider>();
            rootPostIdProvider.Setup(x => x.Get()).Returns(rootId);
            var classUnderTest = new MailItemContextMenuEntry(
                explorer.Object,
                Mock.Of<ISettingsLoadService>(),
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                rootPostIdProvider.Object);

            classUnderTest.OnReplyClick(Mock.Of<IRibbonControl>());

            session.Verify(
                x => x.CreatePost(channelId, ":email: From: sender\n:email: Subject: subject\nmessage", rootId));
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
        public void OnReplyClick_HandlesMattermostExceptionsWhileCreatingPost()
        {
            var postIdProvider = new Mock<IStringProvider>();
            postIdProvider.Setup(x => x.Get()).Returns(string.Empty);
            var post = new Post {root_id = string.Empty};
            var session = new Mock<ISession>();
            session.Setup(x => x.GetPostById(string.Empty)).Returns(post);
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
                postIdProvider.Object);

            classUnderTest.OnReplyClick(Mock.Of<IRibbonControl>());

            errorDisplay.Verify(x => x.Display(It.IsAny<MattermostException>()));
        }

        [Test]
        public void OnReplyClick_UsesPostId_IfRootIdIsEmpty()
        {
            const string postId = "postId";
            const string emptyRootId = "";
            var postIdProvider = new Mock<IStringProvider>();
            postIdProvider.Setup(x => x.Get()).Returns(postId);
            var post = new Post { root_id = emptyRootId };
            var session = new Mock<ISession>();
            session.Setup(x => x.GetPostById(postId)).Returns(post);
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

            session.Verify(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), postId));
        }

        [Test]
        public void OnReplyClick_UsesRootId_IfRootIdIsNotEmpty()
        {
            const string postId = "postId";
            const string notEmptyRootId = "NotEmpty";
            var postIdProvider = new Mock<IStringProvider>();
            postIdProvider.Setup(x => x.Get()).Returns(postId);
            var post = new Post { root_id = notEmptyRootId };
            var session = new Mock<ISession>();
            session.Setup(x => x.GetPostById(postId)).Returns(post);
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

            session.Verify(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), notEmptyRootId));
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