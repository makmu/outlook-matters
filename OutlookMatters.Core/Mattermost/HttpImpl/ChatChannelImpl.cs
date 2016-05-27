using System;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class ChatChannelImpl : IChatChannel
    {
        public string Id
        {
            get { return _channel.ChannelId; }
        }

        public string Name
        {
            get { return _channel.ChannelName; }
        }

        private readonly string _userId;
        private readonly Uri _baseUri;
        private readonly string _token;
        private readonly IRestService _restService;
        private readonly Channel _channel;

        public ChatChannelImpl(IRestService restService, Uri baseUri, string token, string userId, Channel channel)
        {
            _userId = userId;
            _baseUri = baseUri;
            _token = token;
            _restService = restService;
            _channel = channel;
        }

        public void CreatePost(string message)
        {
            var post = new Post
            {
                Id = string.Empty,
                ChannelId = _channel.ChannelId,
                Message = message,
                UserId = _userId,
                RootId = string.Empty
            };
            _restService.CreatePost(_baseUri, _token, _channel.ChannelId, post);
        }
    }
}