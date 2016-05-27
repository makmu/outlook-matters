using System;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpSession : ISession
    {
        private readonly Uri _baseUri;
        private readonly IHttpClient _httpClient;
        private readonly IRestService _restService;
        private readonly string _token;
        private readonly string _userId;

        public HttpSession(Uri baseUri, string token, string userId, IHttpClient httpClient, IRestService restService)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _httpClient = httpClient;
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

        private static MattermostException TranslateException(HttpException hex)
        {
            var error = JsonConvert.DeserializeObject<Interface.Error>(hex.Response.GetPayload());
            var exception = new MattermostException(error);
            return exception;
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
            try
            {
                const string channelsUrl = "api/v1/channels/";
                var getUrl = new Uri(_baseUri, channelsUrl);
                var request = _httpClient.Request(getUrl)
                    .WithContentType("text/json")
                    .WithHeader("Authorization", "Bearer " + _token);
                var response = request.Get();
                var payload = response.GetPayload();
                return JsonConvert.DeserializeObject<ChannelList>(payload);
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }
    }
}