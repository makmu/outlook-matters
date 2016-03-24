using System.Windows.Input;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Settings;

namespace OutlookMatters.Test.Settings
{
    [TestFixture]
    public class SaveCommandTest
    {
        [Test]
        public void CanExecuteIsAlwaysTrue()
        {
            var saveService = new Mock<ISettingsSaveService>();
            var classUnderTest = new SaveCommand(saveService.Object, Mock.Of<IClosableWindow>());

            var result = classUnderTest.CanExecute(null);

            result.Should().BeTrue("because it should always be possible to save");
        }

        [Test]
        public void Execute_CallsSaveService()
        {
            const string mattermostUrl = "http://localhost";
            const string teamId = "teamId";
            const string channelId = "channelId";
            const string username = "username";
            var viewModel = new SettingsViewModel(
                new OutlookMatters.Settings.Settings(string.Empty,string.Empty,string.Empty,string.Empty),
                Mock.Of<ICommand>(),
                Mock.Of<ICommand>())
            {
                MattermostUrl = mattermostUrl,
                TeamId = teamId,
                ChannelId = channelId,
                Username = username
            };
            var saveService = new Mock<ISettingsSaveService>();
            var classUnderTest = new SaveCommand(saveService.Object, Mock.Of<IClosableWindow>());

            classUnderTest.Execute(viewModel);

            saveService.Verify(
                x =>
                    x.Save(
                        It.Is<OutlookMatters.Settings.Settings>(
                            s =>
                                s.MattermostUrl == mattermostUrl && s.TeamId == teamId && s.ChannelId == channelId &&
                                s.Username == username)));
        }

        [Test]
        public void Execute_ClosesWindow()
        {
            var viewModel = new SettingsViewModel(
                new OutlookMatters.Settings.Settings(string.Empty, string.Empty, string.Empty, string.Empty),
                Mock.Of<ICommand>(),
                Mock.Of<ICommand>());
            var window = new Mock<IClosableWindow>();
            var classUnderTest = new SaveCommand(Mock.Of<ISettingsSaveService>(), window.Object);

            classUnderTest.Execute(viewModel);

            window.Verify( x => x.Close() );
        }
    }
}