using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.v3.Interface
{
    public class Error
    {
        [JsonProperty("message")] public string Message;

        [JsonProperty("detailed_error")] public string DetailedError;

        [JsonProperty("request_id")] public string RequestId;

        [JsonProperty("status_code")] public string StatusCode;

        [JsonProperty("is_oauth")] public string IsOauth;
    }
}