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

        protected bool Equals(Channel other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) &&
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
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Type;
                return hashCode;
            }
        }
    }
}
