using FluentAssertions;
using NUnit.Framework;
using OutlookMatters.Mattermost;

namespace OutlookMatters.Test.Mattermost
{
    [TestFixture]
    public class MattermostExceptionTest
    {
        [Test]
        public void Message_ReturnsErrorMessage()
        {
            var error = new OutlookMatters.Mattermost.Error();
            error.message = "error message";
            var classUnderTest = new MattermostException(error);

            var message = classUnderTest.Message;

            message.Should().Be(error.message);
        }

        [Test]
        public void Details_ReturnsDetailedError()
        {
            var error = new OutlookMatters.Mattermost.Error();
            error.detailed_error = "detailed error";
            var classUnderTest = new MattermostException(error);

            var details = classUnderTest.Details;

            details.Should().Be(error.detailed_error);
        }
    }
}
