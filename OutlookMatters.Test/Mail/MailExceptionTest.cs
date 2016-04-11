using FluentAssertions;
using NUnit.Framework;
using OutlookMatters.Core.Mail;

namespace OutlookMatters.Core.Test.Mail
{
    [TestFixture]
    public class MailExceptionTest
    {
        [Test]
        public void Message_ReturnsConstructorInjectedMessage()
        {
            const string message = "message";
            var classUnderTest = new MailException(message);

            var result = classUnderTest.Message;

            result.Should().Be(message);
        }
    }
}