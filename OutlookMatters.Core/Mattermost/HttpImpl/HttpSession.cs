using System;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpSession : ISession
    {
        private readonly Uri _baseUri;
        private readonly IRestService _restService;
        private readonly string _token;
        private readonly string _userId;

        public HttpSession(Uri baseUri, string token, string userId, IRestService restService)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _restService = restService;
        }

        public void CreatePost(string channelId, string message, string rootId = "")
        {
            var post = new Post
            {
                Id = string.Empty,
                ChannelId = channelId,
                Message = message,
                UserId = _userId,
                RootId = rootId
            };
            _restService.CreatePost(_baseUri, _token, channelId, post);
        }

        public Post GetRootPost(string postId)
        {
            var thread = _restService.GetThreadOfPosts(_baseUri, _token, postId);
            var rootId = thread.Posts[postId].RootId;

            if (rootId == "")
            {
                return thread.Posts[postId];
            }

            return thread.Posts[rootId];
        }

        public ChannelList FetchChannelList()
        {
            return _restService.GetChannelList(_baseUri, _token);
        }
    }
}