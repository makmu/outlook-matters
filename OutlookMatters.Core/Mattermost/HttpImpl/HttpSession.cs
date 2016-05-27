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
        private readonly IChatPostFactory _factory;

        public HttpSession(Uri baseUri, string token, string userId, IRestService restService, IChatPostFactory factory)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _restService = restService;
            _factory = factory;
        }

        public void CreatePost(string channelId, string message)
        {
            var post = new Post
            {
                Id = string.Empty,
                ChannelId = channelId,
                Message = message,
                UserId = _userId,
                RootId = string.Empty
            };
            _restService.CreatePost(_baseUri, _token, channelId, post);
        }

        public IChatPost GetPost(string postId)
        {
            var thread = _restService.GetThreadOfPosts(_baseUri, _token, postId);
            return _factory.NewInstance(_baseUri, _token, _userId, thread.Posts[postId]);
        }

        public ChannelList FetchChannelList()
        {
            return _restService.GetChannelList(_baseUri, _token);
        }
    }
}