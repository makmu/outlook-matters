using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OutlookMatters.Core.ContextMenu;
using OutlookMatters.Core.Error;
using OutlookMatters.Core.Mail;
using OutlookMatters.Core.Mattermost.Interface;
using OutlookMatters.Core.Mattermost.Utils;
using OutlookMatters.Core.Reply;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Settings;
using OutlookMatters.Core.Utils;
using Test.OutlookMatters.Core.TestUtils;
using Exception = System.Exception;

namespace Test.OutlookMatters.Core.ContextMenu
{
    [TestFixture]
    public class MailItemContextMenuEntryTest
    {
        private static ISettingsLoadService DefaultSettingsLoadService
        {
            get
            {
                var settings = new AddInSettings("http://localhost", "teamId",
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

        private static MailItem MockMailItemWithMultipleChars(int charValue, string sender = "sender", string subject = "subject")
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < charValue; i++)
            {
                stringBuilder.Append("a");
            }
            var mock = new Mock<MailItem>();
            mock.Setup(m => m.SenderName).Returns(sender);
            mock.Setup(m => m.Subject).Returns(subject);
            mock.Setup(m => m.Body).Returns(stringBuilder.ToString);

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
        public void GetDynamicMenu_ReturnsNoChannelButtons_IfChannelTypeIsDirect()
        {
            const string channelName = "FunnyChannelName";
            const string channelId = "1234";
            const ChannelType directChannel = ChannelType.Direct;
            const string subscribedChannelAttribut = "OnPostIntoChannelClick";
            var channelList = new ChannelList
            {
                Channels =
                    new List<Channel>
                    {
                        new Channel {ChannelName = channelName, ChannelId = channelId, Type = directChannel}
                    }
            };
            var channels = JsonConvert.SerializeObject(channelList);
            var settings = new AddInSettings("http://localhost", "teamId",
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
            const ChannelType publicChannel = ChannelType.Public;
            var channelList = new ChannelList
            {
                Channels =
                    new List<Channel>
                    {
                        new Channel {ChannelName = channelName, ChannelId = channelId, Type = publicChannel}
                    }
            };
            var channels = JsonConvert.SerializeObject(channelList);
            var settings = new AddInSettings("http://localhost", "teamId",
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
            var settings = new AddInSettings("http://localhost", "teamId",
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
            var settings = new AddInSettings(string.Empty, string.Empty,
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
            var settings = new AddInSettings("http://localhost", "teamId",
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
            var sessionCache = new TransientSession(Mock.Of<IClient>(),
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
        public void OnPostIntoChannelClick_CreatesTwoPosts_IfMailIsLongerThanFourThousandChars()
        {
            const string arbitraryChannelPrefrix = "123412341234";
            const int enoughCharsForExcactlyTwoMails = 3980;
            var mail = MockMailItemWithMultipleChars(enoughCharsForExcactlyTwoMails);
            var explorer = new Mock<IMailExplorer>();
            var control = new Mock<IRibbonControl>();
            var payload = new Payload {PostId = It.IsAny<string>()};
            control.Setup(x => x.Id).Returns(arbitraryChannelPrefrix);
            var session = new Mock<ISession>();
            session.Setup(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(payload);
            explorer.Setup(x => x.QuerySelectedMailItem()).Returns(mail);
            var classUnderTest = new MailItemContextMenuEntry(
                explorer.Object,
                Mock.Of<ISettingsLoadService>(),
                Mock.Of<ISettingsSaveService>(),
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnPostIntoChannelClick(control.Object);
            
            session.Verify(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
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
                .Throws(new MattermostException(new Error()));
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
        public void OnRefreshChannelListClick_HandlesAnyExceptionsWhileCreatingPost()
        {
            var control = MockOfRibbonControl();
            var session = new Mock<ISession>();
            session.Setup(x => x.FetchChannelList()).Throws<Exception>();
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(
                MockOfMailExplorer(),
                DefaultSettingsLoadService,
                Mock.Of<ISettingsSaveService>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnRefreshChannelListClick(control);

            errorDisplay.Verify(x => x.Display(It.IsAny<Exception>()));
        }

        [Test]
        public void OnRefreshChannelListClick_HandlesMattermostExceptionWhileFetchingChannels()
        {
            var control = MockOfRibbonControl();
            var session = new Mock<ISession>();
            session.Setup(x => x.FetchChannelList())
                .Throws(new MattermostException(new Error()));
            var errorDisplay = new Mock<IErrorDisplay>();
            var classUnderTest = new MailItemContextMenuEntry(
                MockOfMailExplorer(),
                DefaultSettingsLoadService,
                Mock.Of<ISettingsSaveService>(),
                errorDisplay.Object,
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnRefreshChannelListClick(control);

            errorDisplay.Verify(x => x.Display(It.IsAny<MattermostException>()));
        }

        [Test]
        public void OnRefreshChannelListClick_SavesChannelList()
        {
            const string channelId = "channel id";
            const string channelName = "channel name";
            const ChannelType channelType = ChannelType.Public;
            const string expectedChannelMapResult =
                "{\"channels\":[{\"id\":\"channel id\",\"display_name\":\"channel name\",\"type\":\"O\"}]}";
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
            var saveService = new Mock<ISettingsSaveService>();
            var classUnderTest = new MailItemContextMenuEntry(
                Mock.Of<IMailExplorer>(),
                Mock.Of<ISettingsLoadService>(),
                saveService.Object,
                Mock.Of<IErrorDisplay>(),
                Mock.Of<ISettingsUserInterface>(),
                session.Object,
                Mock.Of<IStringProvider>());

            classUnderTest.OnRefreshChannelListClick(Mock.Of<IRibbonControl>());

            saveService.Verify(x => x.SaveChannels(expectedChannelMapResult));
        }

        [Test]
        public void OnReplyClick_CanHandleUserPermalinkAbort()
        {
            var postIdProvider = new Mock<IStringProvider>();
            postIdProvider.Setup(x => x.Get()).Throws<UserAbortException>();
            var session = new Mock<ISession>();
            var mattermost = new Mock<IClient>();
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
            var rootPost = new Post {id = rootId, channel_id = channelId};
            var session = new Mock<ISession>();
            session.Setup(x => x.GetRootPost(rootId)).Returns(rootPost);
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
            session.Setup(x => x.GetRootPost(string.Empty)).Returns(post);
            session.Setup(x => x.CreatePost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new MattermostException(new Error()));
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