using System.Globalization;
using System.Reflection;
using FluentAssertions;
using Microsoft.Office.Interop.Outlook;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Settings;

namespace Test.OutlookMatters.Core.Settings
{
    [TestFixture]
    public class EnumMatchToBooleanConverterTest
    {
        [Test]
        [TestCase(MattermostVersion.ApiVersionOne, MattermostVersion.ApiVersionOne, true)]
        [TestCase(MattermostVersion.ApiVersionOne, MattermostVersion.ApiVersionThree, false)]
        [TestCase(null, MattermostVersion.ApiVersionThree, false)]
        [TestCase(MattermostVersion.ApiVersionOne, null, false)]
        public void Convert_ConvertsToExpectedResult(object parameter, object value,
            bool expectedResult)
        {
            var classUnderTest = new EnumMatchToBooleanConverter();

            var result = classUnderTest.Convert(value, typeof (bool), parameter, It.IsAny<CultureInfo>());

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
