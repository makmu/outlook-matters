using System;
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

        public ChannelList GetChannelList(Uri uri, string token, string teamId)
        {
            try
            {
                var getUrl = new Uri(uri, "api/v3/teams/" + teamId + "/channels/");
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
    }
}