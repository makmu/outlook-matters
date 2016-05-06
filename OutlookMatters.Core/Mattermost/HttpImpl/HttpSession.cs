﻿using System;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpSession : ISession
    {
        private readonly Uri _baseUri;
        private readonly IHttpClient _httpClient;
        private readonly string _token;
        private readonly string _userId;

        public HttpSession(Uri baseUri, string token, string userId, IHttpClient httpClient)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
            _httpClient = httpClient;
        }

        public Payload CreatePost(string channelId, string message, string rootId = "")
        {
            try
            {
                var post = new Post
                {
                    id = string.Empty,
                    channel_id = channelId,
                    message = message,
                    user_id = _userId,
                    root_id = rootId
                };
                var postUrl = PostUrl(channelId);
                using (var response = _httpClient.Request(postUrl)
                    .WithContentType("text/json")
                    .WithHeader("Authorization", "Bearer " + _token)
                    .Post(JsonConvert.SerializeObject(post)))
                {
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<Payload>(payload);
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }

        private static MattermostException TranslateException(HttpException hex)
        {
            var error = JsonConvert.DeserializeObject<Interface.Error>(hex.Response.GetPayload());
            var exception = new MattermostException(error);
            return exception;
        }

        public Post GetRootPost(string postId)
        {
            try
            {
                var postUrl = "api/v1/posts/" + postId;
                var url = new Uri(_baseUri, postUrl);
                using (var response = _httpClient.Request(url)
                    .WithHeader("Authorization", "Bearer " + _token)
                    .Get())
                {
                    var payload = response.GetPayload();
                    var thread = JsonConvert.DeserializeObject<Thread>(payload);
                    var rootId = thread.posts[postId].root_id;

                    if (rootId == "")
                    {
                        return thread.posts[postId];
                    }

                    return thread.posts[rootId];
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
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

        private Uri PostUrl(string channelId)
        {
            var postUrl = "api/v1/channels/" + channelId + "/create";

            var url = new Uri(_baseUri, postUrl);
            return url;
        }
    }
}