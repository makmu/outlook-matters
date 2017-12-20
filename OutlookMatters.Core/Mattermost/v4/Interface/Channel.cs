using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.v4.Interface
{
    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("display_name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public ChannelType Type { get; set; }
    }
}
