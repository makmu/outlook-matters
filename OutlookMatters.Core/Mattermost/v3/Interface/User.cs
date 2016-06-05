using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.v3.Interface
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        protected bool Equals(User other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }
    }
}