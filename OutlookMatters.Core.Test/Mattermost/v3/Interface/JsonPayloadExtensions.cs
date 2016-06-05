using System.Linq;
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

        public static string SerializeToPayload(this Post post)
        {
            return "{\"id\":\"" + post.Id + "\",\"channel_id\":\"" + post.ChannelId + "\",\"message\":\"" + post.Message +
                   "\",\"user_id\":\"" + post.UserId + "\",\"root_id\":\"" + post.RootId + "\"}";
        }

        public static string SerializeToPayload(this User user)
        {
            return "{\"id\":\"" + user.Id + "\"}";
        }

        public static string SerializeToPayload(this Team team)
        {
            return "{\"id\":\"" + team.Id + "\",\"name\":\"" + team.Name + "\"}";
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

        public static string SerializeToPayload(this InitialLoad initialLoad)
        {
            return
                "{\"teams\":[" + string.Join(",", initialLoad.Teams.Select(x => x.SerializeToPayload())) +
                "]}";
        }

        public static string SerializeToPayload(this Thread post)
        {
            return
                "{\"order\":[\"" + string.Join(",", post.Order) + "\"],\"posts\":{" +
                string.Join(",", post.Posts.Select(x => "\"" + x.Key + "\":" + x.Value.SerializeToPayload())) +
                "}}";
        }
    }
}