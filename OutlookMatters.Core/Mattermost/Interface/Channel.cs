using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public class Channel
    {
        [JsonProperty("id")]
        public string ChannelId { get; set; }

        [JsonProperty("display_name")]
        public string ChannelName { get; set; }

        [JsonProperty("type")]
        public ChannelType Type { get; set; }

        protected bool Equals(Channel other)
        {
            return string.Equals(ChannelId, other.ChannelId) && string.Equals(ChannelName, other.ChannelName) &&
                   Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Channel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ChannelId != null ? ChannelId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ChannelName != null ? ChannelName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Type;
                return hashCode;
            }
        }
    }
}