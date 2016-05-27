using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public struct Thread
    {
        [JsonProperty("order")] public string[] Order;

        [JsonProperty("posts")] public Dictionary<string, Post> Posts;

        public bool Equals(Thread other)
        {
            return Order.Length == other.Order.Length
                   && Order.Intersect(other.Order).Count() == Order.Length
                   && Posts.OrderBy(pair => pair.Key).SequenceEqual(other.Posts.OrderBy(pair => pair.Key));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Thread && Equals((Thread) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Order != null ? Order.GetHashCode() : 0)*397) ^ (Posts != null ? Posts.GetHashCode() : 0);
            }
        }
    }
}