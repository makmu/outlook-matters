using System.Collections.Generic;

namespace OutlookMatters.Core.Mattermost.DataObjects
{
    public struct Thread
    {
        public string[] order;
        public Dictionary<string, Post> posts;
    }
}