using System.Collections.Generic;
using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost
{
    public class ChannelList
    {
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }
    }
}