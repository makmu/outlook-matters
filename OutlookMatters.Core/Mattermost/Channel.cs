using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost
{
    public class Channel
    {
        [JsonProperty("id")]
        public string ChannelId { get; set; }

        [JsonProperty("display_name")]
        public string ChannelName { get; set; }

        [JsonProperty("type")]
        public ChannelType Type { get; set; }
    }
}