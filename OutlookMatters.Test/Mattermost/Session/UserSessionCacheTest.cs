using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Mattermost;
using OutlookMatters.Mattermost.Session;
using OutlookMatters.Security;
using OutlookMatters.Settings;
using System;

namespace OutlookMatters.Test.Mattermost.Session
{
    [TestFixture]
    public class UserSessionCacheTest
    {
        [Test]
        public void GetSession_UsesLoadedSession()
        {
            var settingsLoadService = new Mock<ISettingsLoadService>();
            var settings = new OutlookMatters.Settings.Settings("myUrl", "testTeamId", "myChannel", "Donald Duck");
            settingsLoadService.Setup(x => x.Load()).Returns(settings);
            var mattermost = new Mock<IMattermost>();
            var session = Mock.Of<ISession>();
            mattermost.Setup(
                x => x.LoginByUsername(settings.MattermostUrl, settings.TeamId, settings.Username, It.IsAny<String>()))
                      .Returns(session);
            var classUnderTest = new UserSessionCache(mattermost.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>());

            var returnedSession = classUnderTest.Session;

            returnedSession.Should().BeSameAs(session);
        }

        [Test]
        public void GetSession_UsesLoadedPassword()
        {
            var passwordProvider = new Mock<IPasswordProvider>();
            passwordProvider.Setup(x => x.GetPassword(It.IsAny<String>())).Returns("42");
            var mattermost = new Mock<IMattermost>();
            var session = Mock.Of<ISession>();
            mattermost.Setup(
                x => x.LoginByUsername(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), "42"))
                      .Returns(session);
            var classUnderTest = new UserSessionCache(mattermost.Object,
                DefaultSettingsLoadService,
                passwordProvider.Object);

            var returnedSession = classUnderTest.Session;

            returnedSession.Should().BeSameAs(session);
        }

        [Test]
        public void GetSession_CachesSession()
        {
            var mattermost = new Mock<IMattermost>();
            var session1 = Mock.Of<ISession>();
            var session2 = Mock.Of<ISession>();
            mattermost.SetupSequence(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(session1).Returns(session2);
            var classUnderTest = new UserSessionCache(mattermost.Object,
                DefaultSettingsLoadService,
                Mock.Of<IPasswordProvider>());

            var returnedSession1 = classUnderTest.Session;
            var returnedSession2 = classUnderTest.Session;

            returnedSession1.Should().BeSameAs(returnedSession2);
        }

        [Test]
        public void GetSession_ReturnsNewSession_AfterUpdateTimestampChanged()
        {
            var mattermost = new Mock<IMattermost>();
            var session1 = Mock.Of<ISession>();
            var session2 = Mock.Of<ISession>();
            var settingsLoadService = new Mock<ISettingsLoadService>();
            settingsLoadService.SetupSequence(x => x.LastChanged)
                               .Returns(DateTime.Now)
                               .Returns(DateTime.Now.AddSeconds(1));
            var settings = new OutlookMatters.Settings.Settings("myUrl", "testTeamId", "myChannel", "Donald Duck");
            settingsLoadService.Setup(x => x.Load()).Returns(settings);

            mattermost.SetupSequence(
                x => x.LoginByUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(session1).Returns(session2);
            var classUnderTest = new UserSessionCache(mattermost.Object,
                settingsLoadService.Object,
                Mock.Of<IPasswordProvider>());

            var returnedSession1 = classUnderTest.Session;
            var returnedSession2 = classUnderTest.Session;

            returnedSession1.Should().NotBeSameAs(returnedSession2);
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
    }
}
