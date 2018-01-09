using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public class RestService : IRestService
    {
        private readonly IHttpClient _httpClient;

        public RestService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Login(Uri baseUri, Login login, out string token)
        {
            try
            {
                var loginUrl = new Uri(baseUri, "api/v4/users/login");
                using (var response = _httpClient.Request(loginUrl)
                    .WithContentType("application/json")
                    .Post(JsonConvert.SerializeObject(login)))
                {
                    token = response.GetHeaderValue("Token");
                }
            }
            catch (ServiceException hex)
            {
                throw TranslateException(hex);
            }
        }

        public IEnumerable<Team> GetTeams(Uri baseUri, string token)
        {
            try
            {
                var teamsUrl = new Uri(baseUri, "api/v4/users/me/teams");
                using (var response = _httpClient.Request(teamsUrl)
                    .WithHeader("Authorization", "Bearer " + token)
                    .Get())
                {
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<IEnumerable<Team>>(payload);
                }
            }
            catch (ServiceException hex)
            {
                throw TranslateException(hex);
            }
        }

        public IEnumerable<Channel> GetChannels(Uri baseUri, string token, string teamId)
        {
            try
            {
                var getChannelsUrl = new Uri(baseUri, "api/v4/users/me/teams/" + teamId + "/channels");
                using (var response = _httpClient.Request(getChannelsUrl)
                    .WithHeader("Authorization", "Bearer " + token)
                    .Get())
                {
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<IEnumerable<Channel>>(payload);
                }
            }
            catch (ServiceException hex)
            {
                throw TranslateException(hex);
            }
        }

        public void CreatePost(Uri baseUri, string token, Post newPost)
        {
            try
            {
                var postUrl = new Uri(baseUri, "api/v4/posts");
                using (_httpClient.Request(postUrl)
                    .WithContentType("application/json")
                    .WithHeader("Authorization", "Bearer " + token)
                    .Post(JsonConvert.SerializeObject(newPost)))
                {
                }
            }
            catch (ServiceException hex)
            {
                throw TranslateException(hex);
            }
        }

        public Post GetPostById(Uri baseUri, string token, string postId)
        {
            try
            {
                var postUrl = new Uri(baseUri, "api/v4/posts/" + postId);
                using (var response = _httpClient.Request(postUrl)
                    .WithContentType("application/json")
                    .WithHeader("Authorization", "Bearer " + token)
                    .Get())
                {
                    var payload = response.GetPayload();
                    return JsonConvert.DeserializeObject<Post>(payload);
                }
            }
            catch (ServiceException hex)
            {
                throw TranslateException(hex);
            }
        }

        private static MattermostException TranslateException(ServiceException hex)
        {
            var error = JsonConvert.DeserializeObject<Interface.Error>(hex.Response.GetPayload());
            var exception = new MattermostException(error);
            return exception;
        }
    }
}