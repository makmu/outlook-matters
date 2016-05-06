using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public class Payload
    {
        [JsonProperty("id")]
        public string PostId { get; set; } 
    }
}
