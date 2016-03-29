using System.Collections.Generic;

namespace OutlookMatters.Mattermost.DataObjects
{
    public struct Thread
    {
        public string[] order;
        public Dictionary<string, Post> posts;
    }
}