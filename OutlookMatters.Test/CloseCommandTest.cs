using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Test
{
    [TestFixture]
    public class CloseCommandTest
    {
        [Test]
        public void CanAlwaysExecute()
        {
            var classUnderTest = new CloseCommand(Mock.Of<IClosableWindow>());

            var result = classUnderTest.CanExecute(null);

            result.Should().BeTrue("because it should always be possible to cancel");
        }

        [Test]
        public void Execute_ClosesWindow()
        {
            var window = new Mock<IClosableWindow>();
            var classUnderTest = new CloseCommand(window.Object);

            classUnderTest.Execute(null);

            window.Verify(x => x.Close());
        }
    }
}
