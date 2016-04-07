using System.Collections.Generic;
using Newtonsoft.Json;
namespace OutlookMatters.Mattermost
{
    public class ChannelList
    {
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }
    }
}