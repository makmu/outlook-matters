using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3.Interface;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Mattermost.v3
{
    public class ChatChannelImpl : IChatChannel
    {
        private readonly IRestService _restService;
        private readonly Uri _baseUri;
        private readonly string _token;
        private readonly string _userId;
        private readonly Channel _channel;
        private readonly string _teamId;

        public ChatChannelImpl(IRestService restService, Uri baseUri, string token, string userId, string teamId,
            Channel channel)
        {
            _restService = restService;
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _channel = channel;
            _teamId = teamId;
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
            _restService.CreatePost(_baseUri, _token, _channel.ChannelId, _teamId, post);
        }

        public ChannelSetting ToSetting()
        {
            return new ChannelSetting
            {
                ChannelId = _channel.ChannelId,
                ChannelName = _channel.ChannelName,
                Type = ConvertType(_channel.Type)
            };
        }

        private ChannelTypeSetting ConvertType(ChannelType type)
        {
            switch (type)
            {
                case ChannelType.Direct:
                    return ChannelTypeSetting.Direct;
                case ChannelType.Public:
                    return ChannelTypeSetting.Public;
                case ChannelType.Private:
                    return ChannelTypeSetting.Private;
                default:
                    return ChannelTypeSetting.Public;
            }
        }
    }
}