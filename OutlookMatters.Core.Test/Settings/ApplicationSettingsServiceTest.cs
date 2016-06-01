using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters;
using OutlookMatters.Core.Cache;
using OutlookMatters.Core.Settings;

namespace Test.OutlookMatters.Core.Settings
{
    [TestFixture]
    public class ApplicationSettingsServiceTest
    {
        [Test]
        public void Save_InvalidatesCache()
        {
            var cache = new Mock<ICache>();
            var classUnderTest = new ApplicationSettingsService(cache.Object);

            classUnderTest.SaveCredentials("url42", "teamId42", "username42", MattermostVersion.ApiVersionOne);

            cache.Verify(c => c.Invalidate());
        }

        [Test]
        public void Load_ReturnsSavedSettings()
        {
            var classUnderTest = new ApplicationSettingsService(Mock.Of<ICache>());
            var settings = new AddInSettings("url42", "teamId42", "username42", "channelMap", MattermostVersion.ApiVersionOne);
            classUnderTest.SaveCredentials("url42", "teamId42", "username42", MattermostVersion.ApiVersionOne);
            classUnderTest.SaveChannels("channelMap");

            var loaded = classUnderTest.Load();

            loaded.Should().Be(settings);
        }
    }
}