using FluentAssertions;
using NUnit.Framework;

namespace OutlookMatters.Core.Test.Settings
{
    [TestFixture]
    public class SettingsTest
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
            var otherSettings = new Core.Settings.Settings(url + urlModifier, teamId + teamIdModifier,
                username + usernameModifier, channelMap + channelMapModifier);
            var classUnderTest = new Core.Settings.Settings(url, teamId, username, channelMap);

            var result = classUnderTest.Equals(otherSettings);

            result.Should().Be(expected);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Equals_ReturnsFalse_IfObjectTypeDoesNotMatch(object other)
        {
            var classUnderTest = new Core.Settings.Settings(string.Empty, string.Empty,
                string.Empty, string.Empty);

            var result = classUnderTest.Equals(other);

            result.Should().BeFalse();
        }

        [Test]
        public void Equals_ReturnsTrue_IfReferenceEqual()
        {
            var classUnderTest = new Core.Settings.Settings(string.Empty, string.Empty,
                string.Empty, string.Empty);

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
            var otherSettings = new Core.Settings.Settings(url + urlModifier, teamId + teamIdModifier, username + usernameModifier, channelMap + channelMapModifier);
            var classUnderTest = new Core.Settings.Settings(url, teamId, username, channelMap);

            var result = classUnderTest.GetHashCode() == otherSettings.GetHashCode();

            result.Should().Be(expected);
        }

        [Test]
        public void GetHashCode_ReturnsSameHashCodeIfAllMembersAreNull()
        {
            var otherSettings = new Core.Settings.Settings(null,  null, null, null);
            var classUnderTest = new Core.Settings.Settings(null, null, null, null);

            var result = classUnderTest.GetHashCode();

            result.Should().Be(otherSettings.GetHashCode());
        }
    }
}