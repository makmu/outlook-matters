using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Cache;

namespace Test.OutlookMatters.Core.Cache
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