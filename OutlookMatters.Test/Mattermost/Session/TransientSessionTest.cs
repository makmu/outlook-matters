using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost;
using OutlookMatters.Core.Mattermost.Session;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Test.Mattermost.Session
{
    [TestFixture]
    public class TransientSessionTest
    {
        const string ChannelId = "myChannel";
        const string Message = "message";
        const string RootId = "rootId";
        const string PostId = "postId";

        [Test]
        public void CreatePost_CreatesPostUsingCurrentSession()
        {
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new Core.Settings.Settings("myUrl", "testTeamId", "Donald Duck", "channels");
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var mattermost = new Mock<IMattermost>();
            var session = new Mock<ISession>();
            mattermost.Setup(
                x => x.LoginByUsername(settings.MattermostUrl, settings.TeamId, settings.Username, It.IsAny<string>()))
                .Returns(session.Object);
            var classUnderTest = new TransientSession(mattermost.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>());

            classUnderTest.CreatePost(ChannelId, Message, RootId);

            session.Verify(x => x.CreatePost(ChannelId, Message, RootId));
        }

        [Test]
        public void CreatePost_UsesLoadedPassword()
        {
            const string password = "42";
            var passwordProvider = new Mock<IPasswordProvider>();
            passwordProvider.Setup(x => x.GetPassword(It.IsAny<string>())).Returns(password);
            var mattermost = new Mock<IMattermost>();
            var session = new Mock<ISession>();
            mattermost.Setup(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), password))
                .Returns(session.Object);
            var classUnderTest = new TransientSession(mattermost.Object,
                DefaultSettingsLoadService,
                passwordProvider.Object);

            classUnderTest.CreatePost(ChannelId, Message, RootId);

            session.Verify(x => x.CreatePost(ChannelId, Message, RootId));
        }

        [Test]
        public void CreatePost_CachesSession()
        {
            var mattermost = new Mock<IMattermost>();
            var session1 = new Mock<ISession>();
            var session2 = new Mock<ISession>();
            mattermost.SetupSequence(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(session1.Object)
                .Returns(session2.Object);
            var classUnderTest = new TransientSession(mattermost.Object,
                DefaultSettingsLoadService,
                Mock.Of<IPasswordProvider>());

            classUnderTest.CreatePost(ChannelId, Message, RootId);
            classUnderTest.CreatePost(ChannelId, Message, RootId);

            session1.Verify(x => x.CreatePost(ChannelId, Message, RootId), Times.Exactly(2));
            session2.Verify(x => x.CreatePost(ChannelId, Message, RootId), Times.Never);
        }

        [Test]
        public void CreatePost_UsesNewSession_AfterUpdateTimestampChanged()
        {
            var mattermost = new Mock<IMattermost>();
            var session1 = new Mock<ISession>();
            var session2 = new Mock<ISession>();
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new Core.Settings.Settings("myUrl", "testTeamId", "Donald Duck", "channels");
            settingsLoadService.Setup(x => x.Load()).Returns(settings);

            mattermost.SetupSequence(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(session1.Object)
                .Returns(session2.Object);
            var classUnderTest = new TransientSession(mattermost.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>());

            classUnderTest.CreatePost(ChannelId, Message, RootId);
            classUnderTest.Invalidate();
            classUnderTest.CreatePost(ChannelId, Message, RootId);

            session1.Verify(x => x.CreatePost(ChannelId, Message, RootId), Times.Once);
            session2.Verify(x => x.CreatePost(ChannelId, Message, RootId), Times.Once);
        }

        [Test]
        public void GetPostById_ReturnsPostFromCurrentSession()
        {
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new Core.Settings.Settings("myUrl", "testTeamId", "Donald Duck", "channels");
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var mattermost = new Mock<IMattermost>();
            var session = new Mock<ISession>();
            mattermost.Setup(
                x => x.LoginByUsername(settings.MattermostUrl, settings.TeamId, settings.Username, It.IsAny<string>()))
                .Returns(session.Object);
            var classUnderTest = new TransientSession(mattermost.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>());

            classUnderTest.GetRootPost(PostId);

            session.Verify(x => x.GetRootPost(PostId));
        }

        [Test]
        public void FetchChannelList_ReturnsChannelListFromCurrentSession()
        {
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new Core.Settings.Settings("myUrl", "testTeamId", "Donald Duck", "channels");
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var mattermost = new Mock<IMattermost>();
            var session = new Mock<ISession>();
            mattermost.Setup(
                x => x.LoginByUsername(settings.MattermostUrl, settings.TeamId, settings.Username, It.IsAny<string>()))
                .Returns(session.Object);
            var classUnderTest = new TransientSession(mattermost.Object, settingsLoadService.Object,
                Mock.Of<IPasswordProvider>());

            classUnderTest.FetchChannelList();

            session.Verify(x => x.FetchChannelList());
        }
        
        private static ISettingsLoadService DefaultSettingsLoadService
        {
            get
            {
                var settings = new Core.Settings.Settings("http://localhost", "teamId",
                    "username", "channels");
                var settingsLoadService = new Mock<ISettingsLoadService>();
                settingsLoadService.Setup(x => x.Load()).Returns(settings);
                return settingsLoadService.Object;
            }
        }
    }
}