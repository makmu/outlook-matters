using System;
using System.Collections.Generic;
using System.Linq;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v1.Interface;

namespace OutlookMatters.Core.Mattermost.v1
{
    public class HttpSession : ISession
    {
        private readonly Uri _baseUri;
        private readonly IRestService _restService;
        private readonly string _token;
        private readonly string _userId;
        private readonly IChatPostFactory _factory;
        private readonly IChatChannelFactory _channelFactory;

        public HttpSession(IRestService restService, Uri baseUri, string token, string userId, IChatPostFactory factory,
            IChatChannelFactory channelFactory)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _restService = restService;
            _factory = factory;
            _channelFactory = channelFactory;
        }

        public IChatChannel GetChannel(string channelId)
        {
            return _channelFactory.NewInstance(_restService, _baseUri, _token, _userId,
                new Channel {ChannelId = channelId});
        }

        public IChatPost GetPost(string postId)
        {
            var thread = _restService.GetThreadOfPosts(_baseUri, _token, postId);
            return _factory.NewInstance(_baseUri, _token, _userId, thread.Posts[postId]);
        }

        public IEnumerable<IChatChannel> GetChannels()
        {
            return _restService.GetChannelList(_baseUri, _token).Channels
                .Select(c =>
                    _channelFactory.NewInstance(_restService, _baseUri, _token, _userId, c));
        }
    }
}