using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OutlookMatters.Http;

namespace OutlookMatters.Mattermost.Session
{
    public class UserSession : ISession
    {
        public Channels ChannelList => _channelList;

        private readonly Uri _baseUri;
        private readonly IHttpClient _httpClient;
        private readonly string _token;
        private readonly string _userId;
        private Channels _channelList;

        public UserSession(Uri baseUri, string token, string userId, IHttpClient httpClient)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _httpClient = httpClient;
        }

        public void CreatePost(string channelId, string message, string rootId = "")
        {
            var post = new Post {channel_id = channelId, message = message, user_id = _userId, root_id = rootId};
            var postUrl = PostUrl(channelId);
            _httpClient.Request(postUrl)
                .WithContentType("text/json")
                .WithHeader("Authorization", "Bearer " + _token)
                .PostAndForget(JsonConvert.SerializeObject(post));
        }

        public Post GetPostById(string postId)
        {
            var postUrl = "api/v1/posts/" + postId;
            var url = new Uri(_baseUri, postUrl);
            using (var response = _httpClient.Request(url)
                .WithHeader("Authorization", "Bearer " + _token)
                .Get())
            {
            var thread = JsonConvert.DeserializeObject<PostingThread>(response.GetPayload());
            return thread.posts[postId];
        }
        }

        public void FetchChannelList()
        {
            const string channelsUrl = "api/v1/channels/";
            var getUrl = new Uri(_baseUri, channelsUrl);
            var request = _httpClient.Get(getUrl)
                .WithContentType("text/json")
                .WithHeader("Authorization", "Bearer " + _token);
            var response = request.Get();
            var payload = response.GetPayload();
            _channelList = JsonConvert.DeserializeObject<Channels>(payload);
        }

        private Uri PostUrl(string channelId)
        {
            var postUrl = "api/v1/channels/" + channelId + "/create";

            var url = new Uri(_baseUri, postUrl);
            return url;
        }

        private struct PostingThread
        {
            public string[] order;
            public Dictionary<string, Post> posts;
        }
    }
}