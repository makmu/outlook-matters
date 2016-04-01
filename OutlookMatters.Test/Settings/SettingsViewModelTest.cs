using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Settings;
using System.Windows.Input;

namespace OutlookMatters.Test.Settings
{
    [TestFixture]
    public class SettingsViewModelTest
    {
        [Test]
        public void MattermostUrl_ReturnsUrlFromSettingsProvider()
        {
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username", "channels");
            var classUnderTest = new SettingsViewModel(settings, Mock.Of<ICommand>(), Mock.Of<ICommand>());

            var result = classUnderTest.MattermostUrl;

            result.Should()
                .Be(settings.MattermostUrl, "because the returned url should come from the settings provider");
        }

        [Test]
        public void TeamId_ReturnsTeamIdFromSettingsProvider()
        {
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username", "channels");
            var classUnderTest = new SettingsViewModel(settings, Mock.Of<ICommand>(), Mock.Of<ICommand>());

            var result = classUnderTest.TeamId;

            result.Should()
                .Be(settings.TeamId, "because the returned team id should come from the settings provider");
        }

        [Test]
        public void ChannelId_ReturnsChannelIdFromSettingsProvider()
        {
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username", "channels");
            var classUnderTest = new SettingsViewModel(settings, Mock.Of<ICommand>(), Mock.Of<ICommand>());

            var result = classUnderTest.ChannelId;

            result.Should()
                .Be(settings.ChannelId, "because the returned channel id should come from the settings provider");
        }

        [Test]
        public void Username_ReturnsUsernameFromSettingsProvider()
        {
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username", "channels");
            var classUnderTest = new SettingsViewModel(settings, Mock.Of<ICommand>(), Mock.Of<ICommand>());

            var result = classUnderTest.Username;

            result.Should()
                .Be(settings.Username, "because the returned user name should come from the settings provider");
        }

        [Test]
        public void Save_ReturnsSaveCommand()
        {
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username", "channels");
            var saveCommand = new Mock<ICommand>();
            var classUnderTest = new SettingsViewModel(settings, saveCommand.Object, Mock.Of<ICommand>());

            var result = classUnderTest.Save;

            result.Should().Be(saveCommand.Object, "because the view model should return the save command for binding");
        }

        [Test]
        public void Cancel_ReturnsCancelCommand()
        {
            var settings = new OutlookMatters.Settings.Settings("http://localhost", "teamId", "channelId", "username", "channels");
            var cancelCommand = new Mock<ICommand>();
            var classUnderTest = new SettingsViewModel(settings, Mock.Of<ICommand>(), cancelCommand.Object);

            var result = classUnderTest.Cancel;

            result.Should().Be(cancelCommand.Object, "because the view model should return the cancel command for binding");
        }
    }
}
