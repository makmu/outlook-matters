﻿using System.Linq;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v3.Interface
{
    public static class JsonPayloadExtensions
    {
        public static string SerializeToPayload(this Login login)
        {
            return "{\"login_id\":\"" + login.LoginId + "\",\"password\":\"" + login.Password + "\",\"token\":\"" +
                   login.Token +
                   "\"}";
        }

        public static string SerializeToPayload(this User user)
        {
            return "{\"id\":\"" + user.Id + "\"}";
        }

        public static string SerializeToPayload(this Error error)
        {
            return "{\"message\":\"" + error.Message + "\",\"detailed_error\":\"" + error.DetailedError + "\"}";
        }

        public static string SerializeToPayload(this Channel channel)
        {
            return "{\"id\":\"" + channel.ChannelId +
                   "\",\"create_at\":1458911668852,\"update_at\":1458911668852,\"delete_at\":0,\"type\":\"" +
                   channel.Type +
                   "\",\"display_name\":\"" + channel.ChannelName + "\"}";
        }

        public static string SerializeToPayload(this ChannelList channelList)
        {
            return
                "{\"channels\":[" + string.Join(",", channelList.Channels.Select(x => x.SerializeToPayload())) +
                "]}";
        }
    }
}