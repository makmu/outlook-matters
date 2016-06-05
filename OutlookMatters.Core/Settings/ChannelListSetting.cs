using System.Collections.Generic;
using Newtonsoft.Json;

namespace OutlookMatters.Core.Settings
{
    public class ChannelListSetting
    {
        [JsonProperty("channels")]
        public List<ChannelSetting> Channels { get; set; }
    }
}