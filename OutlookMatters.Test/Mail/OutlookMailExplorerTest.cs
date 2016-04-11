using FluentAssertions;
using Microsoft.Office.Interop.Outlook;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mail;

namespace OutlookMatters.Core.Test.Mail
{
    [TestFixture]
    public class OutlookMailExplorerTest
    {
        [Test]
        public void QuerySelectedMailItem_ReturnsSelectedItem()
        {
            var item = new Mock<MailItem>();
            var selection = new Mock<Selection>();
            selection.Setup(s => s.Count).Returns(2);
            selection.Setup(s => s[1]).Returns(item.Object);
            var explorer = new Mock<Explorer>();
            explorer.Setup(e => e.Selection).Returns(selection.Object);
            var explorerService = new Mock<IExplorerService>();
            explorerService.Setup(e => e.GetActiveExplorer()).Returns(explorer.Object);
            var classUnderTest = new OutlookMailExplorer(explorerService.Object);

            var result = classUnderTest.QuerySelectedMailItem();

            result.Should().Be(item.Object);
        }

        [Test]
        public void QuerySelectedMailItem_Throws_IfExplorerIsNull()
        {
            var explorerService = new Mock<IExplorerService>();
            var classUnderTest = new OutlookMailExplorer(explorerService.Object);

            Assert.Throws<MailException>(() => classUnderTest.QuerySelectedMailItem());
        }

        [Test]
        public void QuerySelectedMailItem_Throws_IfSelectionIsNull()
        {
            var explorer = new Mock<Explorer>();
            var explorerService = new Mock<IExplorerService>();
            explorerService.Setup(e => e.GetActiveExplorer()).Returns(explorer.Object);
            var classUnderTest = new OutlookMailExplorer(explorerService.Object);

            Assert.Throws<MailException>(() => classUnderTest.QuerySelectedMailItem());
        }

        [Test]
        public void QuerySelectedMailItem_Throws_IfSelectionCountIsZero()
        {
            var selection = new Mock<Selection>();
            selection.Setup(s => s.Count).Returns(0);
            var explorer = new Mock<Explorer>();
            explorer.Setup(e => e.Selection).Returns(selection.Object);
            var explorerService = new Mock<IExplorerService>();
            explorerService.Setup(e => e.GetActiveExplorer()).Returns(explorer.Object);
            var classUnderTest = new OutlookMailExplorer(explorerService.Object);

            Assert.Throws<MailException>(() => classUnderTest.QuerySelectedMailItem());
        }

        [Test]
        public void QuerySelectedMailItem_Throws_IfSelectionItemIsNoMailItem()
        {
            var selection = new Mock<Selection>();
            selection.Setup(s => s.Count).Returns(2);
            selection.Setup(s => s[1]).Returns(new object());
            var explorer = new Mock<Explorer>();
            explorer.Setup(e => e.Selection).Returns(selection.Object);
            var explorerService = new Mock<IExplorerService>();
            explorerService.Setup(e => e.GetActiveExplorer()).Returns(explorer.Object);
            var classUnderTest = new OutlookMailExplorer(explorerService.Object);

            Assert.Throws<MailException>(() => classUnderTest.QuerySelectedMailItem());
        }
    }
}