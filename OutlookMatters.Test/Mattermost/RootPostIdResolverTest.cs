using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Mattermost;
using OutlookMatters.Mattermost.DataObjects;
using OutlookMatters.Mattermost.Session;

namespace OutlookMatters.Test.Mattermost
{
    [TestFixture]
    public class RootPostIdResolverTest
    {
        [Test]
        public void Get_ReturnsIdOfFirstPostInThread()
        {
            const string rootId = "rootId";
            const string idWithinThread = "previousReplyId";
            var baseProvider = new Mock<IStringProvider>();
            baseProvider.Setup(x => x.Get()).Returns(idWithinThread);
            var session = new Mock<ISession>();
            var post = new Post {root_id = rootId};
            session.Setup(x => x.GetPostById(idWithinThread)).Returns(post);
            var classUnderTest = new RootPostIdResolver(baseProvider.Object, session.Object);

            var result = classUnderTest.Get();

            result.Should().Be(rootId, "because the resolved id should be that of the first post in thread");
        }

        [Test]
        public void Get_ReturnsIdOfPostUnaltered_IfPostIsAlreadyRoot()
        {
            const string postId = "postId";
            var baseProvider = new Mock<IStringProvider>();
            baseProvider.Setup(x => x.Get()).Returns(postId);
            var session = new Mock<ISession>();
            var post = new Post {root_id = ""};
            session.Setup(x => x.GetPostById(postId)).Returns(post);
            var classUnderTest = new RootPostIdResolver(baseProvider.Object, session.Object);

            var result = classUnderTest.Get();

            result.Should()
                .Be(postId,
                    "because the resolved id should be the one returned from the base provider if base post id is already root");
        }
    }
}