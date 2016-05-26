using System;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
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
                var errorJson = hex.Response.GetPayload();
                var error = JsonConvert.DeserializeObject<Interface.Error>(errorJson);
                throw new MattermostException(error);
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
                var errorJson = hex.Response.GetPayload();
                var error = JsonConvert.DeserializeObject<Interface.Error>(errorJson);
                throw new MattermostException(error);
            }
        }

        public Thread GetPostsThread(Uri baseUri, string token, string postId)
        {
            throw new NotImplementedException();
        }

        public ChannelList GetChannelList(Uri baseUri, string token)
        {
            throw new NotImplementedException();
        }
    }
}