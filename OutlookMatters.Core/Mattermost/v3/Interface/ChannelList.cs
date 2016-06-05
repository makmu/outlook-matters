using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.v3.Interface
{
    public class ChannelList
    {
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }

        protected bool Equals(ChannelList other)
        {
            return Channels.Count == other.Channels.Count
                   && Channels.Intersect(other.Channels).Count() == Channels.Count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChannelList) obj);
        }

        public override int GetHashCode()
        {
            return Channels != null ? Channels.GetHashCode() : 0;
        }
    }
}