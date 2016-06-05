using System.Linq;
using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.v3.Interface
{
    public class InitialLoad
    {
        [JsonProperty("teams")]
        public Team[] Teams { get; set; }

        protected bool Equals(InitialLoad other)
        {
            return Teams.Length == other.Teams.Length
                   && Teams.Intersect(other.Teams).Count() == Teams.Length;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InitialLoad) obj);
        }

        public override int GetHashCode()
        {
            return (Teams != null ? Teams.GetHashCode() : 0);
        }
    }
}