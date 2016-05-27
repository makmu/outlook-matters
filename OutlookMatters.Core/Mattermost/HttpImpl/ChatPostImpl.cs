using System;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class ChatPostImpl : IChatPost
    {
        private readonly IRestService _restService;
        private readonly Uri _baseUri;
        private readonly string _token;
        private readonly string _userId;
        private readonly Post _post;

        public ChatPostImpl(IRestService restService, Uri baseUri, string token, string userId, Post post)
        {
            _restService = restService;
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _post = post;
        }

        public void Reply(string message)
        {
            var rootId = _post.RootId == "" ? _post.Id : _post.RootId;
            var newPost = new Post
            {
                Id = string.Empty,
                ChannelId = _post.ChannelId,
                Message = message,
                UserId = _userId,
                RootId = rootId
            };
            _restService.CreatePost(_baseUri, _token, _post.ChannelId, newPost);
        }
    }
}