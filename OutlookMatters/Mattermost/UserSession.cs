using System;
using Newtonsoft.Json;
using OutlookMatters.Http;

namespace OutlookMatters.Mattermost
{
    public class UserSession: ISession
    {
        private readonly Uri _baseUri;
        private readonly string _token;
        private readonly string _userId;
        private readonly IHttpClient _httpClient;

        public UserSession(Uri baseUri, string token, string userId, IHttpClient httpClient)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _httpClient = httpClient;
        }

        public void CreatePost(string channelId, string message)
        {
            var post = new Post {channel_id = channelId, message = message, user_id = _userId};
            var postUrl = PostUrl(channelId);
            _httpClient.Post(postUrl)
                .WithContentType("text/json")
                .WithHeader("Authorization", "Bearer " + _token)
                .Send(JsonConvert.SerializeObject(post));

        }

        private Uri PostUrl(string channelId)
        {
            string postUrl = "api/v1/channels/" + channelId + "/create";

            var url = new Uri(_baseUri, postUrl);
            return url;
        }
    }
}
