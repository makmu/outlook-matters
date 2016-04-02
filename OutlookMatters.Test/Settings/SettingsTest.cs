using FluentAssertions;
using NUnit.Framework;

namespace OutlookMatters.Test.Settings
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
        public void Equals_ChecksAllMembers(string urlModifier, string teamIdModifier, string channelIdModifier,
            string usernameModifier, bool expected)
        {
            const string url = "http://tempuri.org";
            const string teamId = "team id";
            const string channelId = "channel id";
            const string username = "user name";
            var otherSettings = new OutlookMatters.Settings.Settings(url + urlModifier, teamId + teamIdModifier,
                channelId + channelIdModifier, username + usernameModifier);
            var classUnderTest = new OutlookMatters.Settings.Settings(url, teamId, channelId, username);

            var result = classUnderTest.Equals(otherSettings);

            result.Should().Be(expected);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Equals_ReturnsFalse_IfObjectTypeDoesNotMatch(object other)
        {
            var classUnderTest = new OutlookMatters.Settings.Settings(string.Empty, string.Empty, string.Empty,
                string.Empty);

            var result = classUnderTest.Equals(other);

            result.Should().BeFalse();
        }

        [Test]
        public void Equals_ReturnsTrue_IfReferenceEqual()
        {
            var classUnderTest = new OutlookMatters.Settings.Settings(string.Empty, string.Empty, string.Empty,
                string.Empty);

            var result = classUnderTest.Equals(classUnderTest);

            result.Should().BeTrue();
        }

        [Test]
        [TestCase("", "", "", "", true)]
        [TestCase("x", "", "", "", false)]
        [TestCase("", "x", "", "", false)]
        [TestCase("", "", "x", "", false)]
        [TestCase("", "", "", "x", false)]
        public void GetHashCode_(string urlModifier, string teamIdModifier, string channelIdModifier,
            string usernameModifier, bool expected)
        {
            const string url = "http://tempuri.org";
            const string teamId = "team id";
            const string channelId = "channel id";
            const string username = "user name";
            var otherSettings = new OutlookMatters.Settings.Settings(url + urlModifier, teamId + teamIdModifier,
                channelId + channelIdModifier, username + usernameModifier);
            var classUnderTest = new OutlookMatters.Settings.Settings(url, teamId, channelId, username);

            var result = classUnderTest.GetHashCode() == otherSettings.GetHashCode();

            result.Should().Be(expected);
        }
    }
}