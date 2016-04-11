using System;
using System.Windows.Input;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Test.Settings
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
            const string username = "username";
            var viewModel = new SettingsViewModel(
                new Core.Settings.Settings(string.Empty, string.Empty, string.Empty,
                    string.Empty),
                Mock.Of<ICommand>(),
                Mock.Of<ICommand>())
            {
                MattermostUrl = mattermostUrl,
                TeamId = teamId,
                Username = username
            };
            var saveService = new Mock<ISettingsSaveService>();
            var classUnderTest = new SaveCommand(saveService.Object, Mock.Of<IClosableWindow>());

            classUnderTest.Execute(viewModel);

            saveService.Verify(x => x.SaveCredentials(mattermostUrl, teamId, username));
        }

        [Test]
        public void Execute_ClosesWindow()
        {
            var viewModel = new SettingsViewModel(
                new Core.Settings.Settings(string.Empty, string.Empty, string.Empty,
                    string.Empty),
                Mock.Of<ICommand>(),
                Mock.Of<ICommand>());
            var window = new Mock<IClosableWindow>();
            var classUnderTest = new SaveCommand(Mock.Of<ISettingsSaveService>(), window.Object);

            classUnderTest.Execute(viewModel);

            window.Verify(x => x.Close());
        }

        [Test]
        public void Execute_Throws_IfArgumentIsNotViewModel()
        {
            var classUnderTest = new SaveCommand(Mock.Of<ISettingsSaveService>(), Mock.Of<IClosableWindow>());

            Assert.Throws<ArgumentException>(() => classUnderTest.Execute(new object()));
        }
    }
}