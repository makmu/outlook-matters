using Newtonsoft.Json;

namespace OutlookMatters.Core.Settings
{
    public class ChannelSetting
    {
        [JsonProperty("id")]
        public string ChannelId { get; set; }

        [JsonProperty("display_name")]
        public string ChannelName { get; set; }

        [JsonProperty("type")]
        public ChannelTypeSetting Type { get; set; }
    }
}