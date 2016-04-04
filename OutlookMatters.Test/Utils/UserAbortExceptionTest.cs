using FluentAssertions;
using NUnit.Framework;
using OutlookMatters.Utils;

namespace OutlookMatters.Test.Utils
{
    [TestFixture]
    public class UserAbortExceptionTest
    {
        [Test]
        public void Message_ReturnsConstructorInjectedMessage()
        {
            const string message = "message";
            var classUnderTest = new UserAbortException(message);

            var result = classUnderTest.Message;

            result.Should().Be(message);
        }
    }
}