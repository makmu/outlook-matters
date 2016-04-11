using System.Collections.Generic;
using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public class ChannelList
    {
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }
    }
}