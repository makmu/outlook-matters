using FluentAssertions;
using NUnit.Framework;
using OutlookMatters.Settings;

namespace OutlookMatters.Test.Settings
{
    [TestFixture]
    public class ApplicationSettingsServiceTest
    {
        [Test]
        public void Save_UpdatesChangedTimestamp()
        {
            var service = new ApplicationSettingsService();
            var initialLastChanged = service.LastChanged;  

            service.Save(new OutlookMatters.Settings.Settings("url", "teamId", "channelId", "username"));

            service.LastChanged.Should().BeAfter(initialLastChanged);
        }

        [Test]
        public void Load_DoesntUpdateChangedTimestamp()
        {
            var service = new ApplicationSettingsService();
            var initialLastChanged = service.LastChanged;

            service.Load();

            service.LastChanged.Should().BeSameDateAs(initialLastChanged);
        }

        [Test]
        public void Load_ReturnsSavedSettings()
        {
            var service = new ApplicationSettingsService();
            var settings = new OutlookMatters.Settings.Settings("url42", "teamId42", "channelId42", "username42");
            service.Save(settings);

            var loaded = service.Load();

            loaded.Should().Be(settings);
        }
    }
}
