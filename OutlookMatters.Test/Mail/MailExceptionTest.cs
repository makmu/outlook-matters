using FluentAssertions;
using NUnit.Framework;
using OutlookMatters.Mail;

namespace OutlookMatters.Test.Mail
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