using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Mattermost.Session;
using OutlookMatters.Settings;

namespace OutlookMatters.Test.Settings
{
    [TestFixture]
    public class ApplicationSettingsServiceTest
    {
        [Test]
        public void Save_InvalidatesCache()
        {
            var cache = new Mock<ICache>();
            var settings = new OutlookMatters.Settings.Settings("url42", "teamId42", "channelId42", "username42", "channels");
            var classUnderTest = new ApplicationSettingsService(cache.Object);

            classUnderTest.Save(settings);

            cache.Verify(c => c.Invalidate());
        }

        [Test]
        public void Load_ReturnsSavedSettings()
        {
            var classUnderTest = new ApplicationSettingsService(Mock.Of<ICache>());
            var settings = new OutlookMatters.Settings.Settings("url42", "teamId42", "channelId42", "username42", "channels");
            classUnderTest.Save(settings);

            var loaded = classUnderTest.Load();

            loaded.Should().Be(settings);
        }
    }
}