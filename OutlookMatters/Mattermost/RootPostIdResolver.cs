using OutlookMatters.Mattermost.Session;

namespace OutlookMatters.Mattermost
{
    public class RootPostIdResolver : IStringProvider
    {
        private readonly IStringProvider _baseProvider;
        private readonly ISession _session;

        public RootPostIdResolver(IStringProvider baseProvider, ISession session)
        {
            _session = session;
            _baseProvider = baseProvider;
        }

        public string Get()
        {
            var postId = _baseProvider.Get();
            var rootPost = _session.GetPostById(postId);
            if (rootPost.root_id == "")
            {
                return postId;
            }
            return rootPost.root_id;
        }
    }
}