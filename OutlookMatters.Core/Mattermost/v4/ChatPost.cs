using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public class ChatPost : IChatPost
    {
        private readonly IRestService _restService;
        private readonly Uri _baseUri;
        private readonly string _token;
        private readonly string _teamId;
        private readonly Post _post;

        public ChatPost(IRestService restService, Uri baseUri, string token, string teamId, Post post)
        {
            _restService = restService;
            _baseUri = baseUri;
            _token = token;
            _teamId = teamId;
            _post = post;
        }

        public void Reply(string message)
        {
            var rootId = _post.RootId == "" ? _post.Id : _post.RootId;
            var newPost = new Post
            {
                ChannelId = _post.ChannelId,
                Message = message,
                RootId = rootId
            };
            _restService.CreatePost(_baseUri, _token, newPost);
        }
    }
}
