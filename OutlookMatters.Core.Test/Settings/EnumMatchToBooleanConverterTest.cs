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
        [TestCase(MattermostVersion.ApiVersionFour, MattermostVersion.ApiVersionFour, true)]
        [TestCase(MattermostVersion.ApiVersionThree, MattermostVersion.ApiVersionFour, false)]
        [TestCase(null, MattermostVersion.ApiVersionFour, false)]
        [TestCase(MattermostVersion.ApiVersionThree, null, false)]
        public void Convert_ConvertsToExpectedResult(object parameter, object value,
            bool expectedResult)
        {
            var classUnderTest = new EnumMatchToBooleanConverter();

            var result = classUnderTest.Convert(value, typeof (bool), parameter, It.IsAny<CultureInfo>());

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(true, "ApiVersionFour", MattermostVersion.ApiVersionFour)]
        [TestCase(false, "ApiVersionFour", null)]
        [TestCase(true, null, null)]
        [TestCase(null, "ApiVersionFour", null)]
        public void ConvertBack_ConvertsToExpectedResult(object value, object parameter, object expectedResult)
        {
            var classUnderTest = new EnumMatchToBooleanConverter();

            var result = classUnderTest.ConvertBack(value, typeof(MattermostVersion), parameter, It.IsAny<CultureInfo>());

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
