﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace OutlookMatters.Core.Mattermost.v3
{
    public class RestServiceImpl : IRestService
    {
        private readonly IHttpClient _httpClient;

        public RestServiceImpl(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public User Login(Uri baseUri, Login login, out string token)
        {
            try
            {
                var loginUrl = new Uri(baseUri, "api/v3/users/login");
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

        public InitialLoad GetInitialLoad(Uri baseUri, string token)
        {
            try
            {
                var getUrl = new Uri(baseUri, "api/v3/users/initial_load");
                using (
                    var response = _httpClient.Request(getUrl)
                        .WithHeader("Authorization", "Bearer " + token)
                        .Get()
                    )
                {
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<InitialLoad>(payload);
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

        public IEnumerable<Channel> GetChannelList(Uri uri, string token, string teamGuid)
        {
            try
            {
                var getUrl = new Uri(uri, "api/v3/teams/" + teamGuid + "/channels/");
                using (var response = _httpClient.Request(getUrl)
                    .WithHeader("Authorization", "Bearer " + token)
                    .Get())
                {
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<IEnumerable<Channel>>(payload);
                }
            }
            catch (HttpException hex)
            {
                throw TranslateException(hex);
            }
        }

        public void CreatePost(Uri baseUri, string token, string channelId, string teamGuid, Post newPost)
        {
            try
            {
                var postUrl = new Uri(baseUri, "api/v3/teams/" + teamGuid + "/channels/" + channelId + "/posts/create");
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

        public Thread GetPostById(Uri baseUri, string token, string teamGuid, string postId)
        {
            try
            {
                var getUrl = new Uri(baseUri, "api/v3/teams/" + teamGuid + "/posts/" + postId);
                using (var response = _httpClient.Request(getUrl)
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
    }
}