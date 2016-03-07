using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlookMatters.Settings
{
    public class Settings
    {
        public string MattermostUrl { get; }
        public string TeamId { get; }
        public string ChannelId { get; }
        public string Username { get; }

        public Settings(string mattermostUrl, string teamId, string channelId, string username)
        {
            MattermostUrl = mattermostUrl;
            TeamId = teamId;
            ChannelId = channelId;
            Username = username;
        }
    }
}
