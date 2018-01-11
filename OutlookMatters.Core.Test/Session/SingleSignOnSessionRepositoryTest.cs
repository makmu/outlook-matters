using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Session;
using OutlookMatters.Core.Settings;
using OutlookMatters.Core.Utils;

namespace Test.OutlookMatters.Core.Session
{
    [TestFixture]
    public class SingleSignOnSessionRepositoryTest
    {
        [Test]
        public async Task RestoreSession_ReturnsNewSessionFromClient()
        {
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new AddInSettings("myUrl", "testTeamId", "Donald Duck", "channels",
                It.IsAny<MattermostVersion>());
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var mattermost = new Mock<IClient>();
            var clientFactory = SetupClientFactoryMock(mattermost);
            var session = new Mock<ISession>();
            mattermost.Setup(
                    x => x.LoginByUsername(settings.MattermostUrl, settings.TeamId, settings.Username,
                        It.IsAny<string>()))
                .Returns(session.Object);
            var classUnderTest = new SingleSignOnSessionRepository(clientFactory.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>(),
                Mock.Of<ITrustInvalidSslQuestion>(),
                Mock.Of<IServerCertificateValidator>());

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
            var clientFactory = SetupClientFactoryMock(mattermost);
            mattermost.Setup(
                    x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), password))
                .Returns(session.Object);
            var classUnderTest = new SingleSignOnSessionRepository(clientFactory.Object,
                DefaultSettingsLoadService,
                passwordProvider.Object,
                Mock.Of<ITrustInvalidSslQuestion>(),
                Mock.Of<IServerCertificateValidator>());

            var result = await classUnderTest.RestoreSession();

            result.Should().Be(session.Object);
        }

        [Test]
        public async Task RestoreSession_HasCachingSemantics()
        {
            var mattermost = new Mock<IClient>();
            var session1 = new Mock<ISession>();
            var session2 = new Mock<ISession>();
            var clientFactory = SetupClientFactoryMock(mattermost);
            mattermost.SetupSequence(
                    x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(session1.Object)
                .Returns(session2.Object);
            var classUnderTest = new SingleSignOnSessionRepository(clientFactory.Object,
                DefaultSettingsLoadService,
                Mock.Of<IPasswordProvider>(),
                Mock.Of<ITrustInvalidSslQuestion>(),
                Mock.Of<IServerCertificateValidator>());

            var result1 = await classUnderTest.RestoreSession();
            var result2 = await classUnderTest.RestoreSession();

            result1.Should().Be(session1.Object);
            result2.Should().Be(session1.Object);
        }

        [Test]
        public async Task RestoreSession_RetriesLogin_IfWebExceptionIsTrustFailureAndUserContinues()
        {
            var mattermost = new Mock<IClient>();
            var session = new Mock<ISession>();
            var clientfactory = SetupClientFactoryMock(mattermost);
            var exception = new WebException("", new Exception("invalid ssl"), WebExceptionStatus.TrustFailure, It.IsAny<HttpWebResponse>() );
            mattermost.SetupSequence(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                    .Throws(exception)
                    .Returns(session.Object);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new AddInSettings("myUrl", "testTeamId", "Donald Duck", "channels",
                MattermostVersion.ApiVersionFour);
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var invalidSslQuestion = new Mock<ITrustInvalidSslQuestion>();
            invalidSslQuestion.Setup(x => x.GetAnswer(settings.MattermostUrl, exception.InnerException.Message)).Returns(true);
            var cut = new SingleSignOnSessionRepository(clientfactory.Object, settingsLoadService.Object,
                new Mock<IPasswordProvider>().Object, invalidSslQuestion.Object,
                new Mock<IServerCertificateValidator>().Object);

            var result = await cut.RestoreSession();

            result.ShouldBeEquivalentTo(session.Object);
        }

        [Test]
        public void RestoreSession_ThrowsUserAbortException_IfUserAbortsOnTrustFailureException()
        {
            var mattermost = new Mock<IClient>();
            var session = new Mock<ISession>();
            var clientfactory = SetupClientFactoryMock(mattermost);
            var exception = new WebException("", new Exception("invalid ssl"), WebExceptionStatus.TrustFailure, It.IsAny<HttpWebResponse>() );
            mattermost.SetupSequence(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                    .Throws(exception)
                    .Returns(session.Object);
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new AddInSettings("myUrl", "testTeamId", "Donald Duck", "channels",
                MattermostVersion.ApiVersionFour);
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var invalidSslQuestion = new Mock<ITrustInvalidSslQuestion>();
            invalidSslQuestion.Setup(x => x.GetAnswer(settings.MattermostUrl, exception.InnerException.Message)).Returns(false);
            var cut = new SingleSignOnSessionRepository(clientfactory.Object, settingsLoadService.Object,
                new Mock<IPasswordProvider>().Object, invalidSslQuestion.Object,
                new Mock<IServerCertificateValidator>().Object);

            Func<Task> func = async () => { await cut.RestoreSession(); };

            func.ShouldThrow<UserAbortException>();
        }

        [Test]
        public async Task RestoreSession_CreatesNewSession_IfCacheInvalidated()
        {
            var mattermost = new Mock<IClient>();
            var session1 = new Mock<ISession>();
            var session2 = new Mock<ISession>();
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new AddInSettings("myUrl", "testTeamId", "Donald Duck", "channels",
                MattermostVersion.ApiVersionFour);
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var clientFactory = SetupClientFactoryMock(mattermost);
            mattermost.SetupSequence(
                    x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(session1.Object)
                .Returns(session2.Object);
            var classUnderTest = new SingleSignOnSessionRepository(clientFactory.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>(),
                Mock.Of<ITrustInvalidSslQuestion>(),
                Mock.Of<IServerCertificateValidator>());

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
                    "username", "channels", MattermostVersion.ApiVersionFour);
                var settingsLoadService = new Mock<ISettingsLoadService>();
                settingsLoadService.Setup(x => x.Load()).Returns(settings);
                return settingsLoadService.Object;
            }
        }

        private Mock<IClientFactory> SetupClientFactoryMock(Mock<IClient> mattermost)
        {
            var clientFactory = new Mock<IClientFactory>();
            clientFactory.Setup(x => x.GetClient(It.IsAny<MattermostVersion>())).Returns(mattermost.Object);
            return clientFactory;
        }
    }
}