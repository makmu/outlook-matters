using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Mattermost.Session;

namespace OutlookMatters.Core.Test.Mattermost.Session
{
    [TestFixture]
    public class CompositeCacheTest
    {
        [Test]
        public void Test()
        {
            var cache = new Mock<ICache>();
            var classUnderTest = new CompositeCache();
            classUnderTest.Add(cache.Object);

            classUnderTest.Invalidate();

            cache.Verify(c => c.Invalidate());
        }
    }
}