using FluentAssertions;
using NUnit.Framework;

namespace OutlookMatters.Test
{
    [TestFixture]
    public class MailItemContextMenuEntryTest
    {
        [Test]
        public void GetCustomUI_ReturnsCustomUiForExplorer()
        {
            var classUnderTest = new MailItemContextMenuEntry();

            var result = classUnderTest.GetCustomUI("Microsoft.Outlook.Explorer");

            result.Should().NotBeEmpty("because there should be custom UI xml for the outlook explorer");
        }
    }
}
