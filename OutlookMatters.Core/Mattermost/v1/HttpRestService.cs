using System;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.v1.Interface;

namespace OutlookMatters.Core.Mattermost.v1
{
    public class HttpRestService : IRestService
    {
        private readonly IHttpClient _httpClient;

        public HttpRestService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public User Login(Uri baseUri, Login login, out string token)
        {
            try
            {
                var loginUrl = new Uri(baseUri, "api/v1/users/login");
                using (var response = _httpClient.Request(loginUrl)
                    .WithContentType("text/json")
                    .Post(JsonConvert.SerializeObject(login)))
                {
                    token = response.GetHeaderValue("Token");
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<User>(payload);
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }

        public void CreatePost(Uri baseUri, string token, string channelId, Post newPost)
        {
            try
            {
                var postUrl = new Uri(baseUri, "api/v1/channels/" + channelId + "/create");
                using (_httpClient.Request(postUrl)
                    .WithContentType("text/json")
                    .WithHeader("Authorization", "Bearer " + token)
                    .Post(JsonConvert.SerializeObject(newPost)))
                {
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }

        public Thread GetThreadOfPosts(Uri baseUri, string token, string postId)
        {
            try
            {
                var postUrl = "api/v1/posts/" + postId;
                var url = new Uri(baseUri, postUrl);
                using (var response = _httpClient.Request(url)
                    .WithHeader("Authorization", "Bearer " + token)
                    .Get())
                {
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<Thread>(payload);
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }

        public ChannelList GetChannelList(Uri baseUri, string token)
        {
            try
            {
                var getUrl = new Uri(baseUri, "api/v1/channels/");
                using (var response = _httpClient.Request(getUrl)
                    .WithHeader("Authorization", "Bearer " + token)
                    .Get())
                {
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<ChannelList>(payload);
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }

        private static MattermostException TranslateException(HttpException hex)
        {
            var error = JsonConvert.DeserializeObject<v1.Interface.Error>(hex.Response.GetPayload());
            var exception = new MattermostException(error);
            return exception;
        }
    }
}