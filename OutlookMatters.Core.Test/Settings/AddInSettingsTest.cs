using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Settings;

namespace Test.OutlookMatters.Core.Settings
{
    [TestFixture]
    public class AddInSettingsTest
    {
        [Test]
        [TestCase("", "", "", "", true)]
        [TestCase("x", "", "", "", false)]
        [TestCase("", "x", "", "", false)]
        [TestCase("", "", "x", "", false)]
        [TestCase("", "", "", "x", false)]
        public void Equals_ChecksAllMembers(string urlModifier, string teamIdModifier,
            string usernameModifier, string channelMapModifier, bool expected)
        {
            const string url = "http://tempuri.org";
            const string teamId = "team id";
            const string username = "user name";
            const string channelMap = "channel map";
            const MattermostVersion version = MattermostVersion.ApiVersionFour;
            var otherSettings = new AddInSettings(url + urlModifier, teamId + teamIdModifier,
                username + usernameModifier, channelMap + channelMapModifier, version);
            var classUnderTest = new AddInSettings(url, teamId, username, channelMap, version);

            var result = classUnderTest.Equals(otherSettings);

            result.Should().Be(expected);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Equals_ReturnsFalse_IfObjectTypeDoesNotMatch(object other)
        {
            var classUnderTest = new AddInSettings(string.Empty, string.Empty,
                string.Empty, string.Empty, It.IsAny<MattermostVersion>());

            var result = classUnderTest.Equals(other);

            result.Should().BeFalse();
        }

        [Test]
        public void Equals_ReturnsTrue_IfReferenceEqual()
        {
            var classUnderTest = new AddInSettings(string.Empty, string.Empty,
                string.Empty, string.Empty, It.IsAny<MattermostVersion>());

            var result = classUnderTest.Equals(classUnderTest);

            result.Should().BeTrue();
        }

        [Test]
        [TestCase("", "", "", "", true)]
        [TestCase("x", "", "", "", false)]
        [TestCase("", "x", "", "", false)]
        [TestCase("", "", "x", "", false)]
        [TestCase("", "", "", "x", false)]
        public void GetHashCode_CalculatesHashBasedOnMembers(string urlModifier, string teamIdModifier,
            string usernameModifier, string channelMapModifier, bool expected)
        {
            const string url = "http://tempuri.org";
            const string teamId = "team id";
            const string username = "user name";
            const string channelMap = "channel map";
            const MattermostVersion version = MattermostVersion.ApiVersionFour;
            var otherSettings = new AddInSettings(url + urlModifier, teamId + teamIdModifier,
                username + usernameModifier, channelMap + channelMapModifier, version);
            var classUnderTest = new AddInSettings(url, teamId, username, channelMap, version);

            var result = classUnderTest.GetHashCode() == otherSettings.GetHashCode();

            result.Should().Be(expected);
        }

        [Test]
        public void GetHashCode_ReturnsSameHashCodeIfAllMembersAreNull()
        {
            var otherSettings = new AddInSettings(null, null, null, null, It.IsAny<MattermostVersion>());
            var classUnderTest = new AddInSettings(null, null, null, null, It.IsAny<MattermostVersion>());

            var result = classUnderTest.GetHashCode();

            result.Should().Be(otherSettings.GetHashCode());
        }
    }
}