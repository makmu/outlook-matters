using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost.Interface;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Session;
using OutlookMatters.Core.Settings;

namespace Test.OutlookMatters.Core.Session
{
    [TestFixture]
    public class SingleSignOnSessionRepositoryTest
    {
        [Test]
        public async Task RestoreSession_ReturnsNewSessionFromClient()
        {
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new AddInSettings("myUrl", "testTeamId", "Donald Duck", "channels");
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var mattermost = new Mock<IClient>();
            var session = new Mock<ISession>();
            mattermost.Setup(
                x => x.LoginByUsername(settings.MattermostUrl, settings.TeamId, settings.Username, It.IsAny<string>()))
                .Returns(session.Object);
            var classUnderTest = new SingleSignOnSessionRepository(mattermost.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>());

            var result = await classUnderTest.RestoreSession();

            result.Should().Be(session.Object);
        }

        [Test]
        public async Task RestoreSession_UsesProvidedPassword()
        {
            const string password = "42";
            var passwordProvider = new Mock<IPasswordProvider>();
            passwordProvider.Setup(x => x.GetPassword(It.IsAny<string>())).Returns(password);
            var mattermost = new Mock<IClient>();
            var session = new Mock<ISession>();
            mattermost.Setup(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), password))
                .Returns(session.Object);
            var classUnderTest = new SingleSignOnSessionRepository(mattermost.Object,
                DefaultSettingsLoadService,
                passwordProvider.Object);

            var result = await classUnderTest.RestoreSession();

            result.Should().Be(session.Object);
        }

        [Test]
        public async Task RestoreSession_HasCachingSemantics()
        {
            var mattermost = new Mock<IClient>();
            var session1 = new Mock<ISession>();
            var session2 = new Mock<ISession>();
            mattermost.SetupSequence(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(session1.Object)
                .Returns(session2.Object);
            var classUnderTest = new SingleSignOnSessionRepository(mattermost.Object,
                DefaultSettingsLoadService,
                Mock.Of<IPasswordProvider>());

            var result1 = await classUnderTest.RestoreSession();
            var result2 = await classUnderTest.RestoreSession();

            result1.Should().Be(session1.Object);
            result2.Should().Be(session1.Object);
        }

        [Test]
        public async Task RestoreSession_CreatesNewSession_IfCacheInvalidated()
        {
            var mattermost = new Mock<IClient>();
            var session1 = new Mock<ISession>();
            var session2 = new Mock<ISession>();
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new AddInSettings("myUrl", "testTeamId", "Donald Duck", "channels");
            settingsLoadService.Setup(x => x.Load()).Returns(settings);

            mattermost.SetupSequence(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(session1.Object)
                .Returns(session2.Object);
            var classUnderTest = new SingleSignOnSessionRepository(mattermost.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>());

            var result1 = await classUnderTest.RestoreSession();
            classUnderTest.Invalidate();
            var result2 = await classUnderTest.RestoreSession();

            result1.Should().Be(session1.Object);
            result2.Should().Be(session2.Object);
        }

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
    }
}