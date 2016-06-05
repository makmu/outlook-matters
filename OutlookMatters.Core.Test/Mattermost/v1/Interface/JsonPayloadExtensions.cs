using System.Linq;
using OutlookMatters.Core.Mattermost.v1.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v1.Interface
{
    public static class JsonPayloadExtensions
    {
        public static string SerializeToPayload(this Login login)
        {
            return "{\"name\":\"" + login.Name + "\",\"email\":\"" + login.Email + "\",\"password\":\"" + login.Password +
                   "\"}";
        }

        public static string SerializeToPayload(this User user)
        {
            return "{\"id\":\"" + user.Id + "\"}";
        }

        public static string SerializeToPayload(this Error error)
        {
            return "{\"message\":\"" + error.message + "\",\"detailed_error\":\"" + error.detailed_error + "\"}";
        }

        public static string SerializeToPayload(this Post post)
        {
            return "{\"id\":\"" + post.Id + "\",\"channel_id\":\"" + post.ChannelId + "\",\"message\":\"" + post.Message +
                   "\",\"user_id\":\"" + post.UserId + "\",\"root_id\":\"" + post.RootId + "\"}";
        }

        public static string SerializeToPayload(this Channel channel)
        {
            return "{\"id\":\"" + channel.ChannelId +
                   "\",\"create_at\":1458911668852,\"update_at\":1458911668852,\"delete_at\":0,\"type\":\"" +
                   channel.Type +
                   "\",\"display_name\":\"" + channel.ChannelName + "\"}";
        }

        public static string SerializeToPayload(this Thread post)
        {
            return
                "{\"order\":[\"" + string.Join(",", post.Order) + "\"],\"posts\":{" +
                string.Join(",", post.Posts.Select(x => "\"" + x.Key + "\":" + SerializeToPayload((Post) x.Value))) +
                "}}";
        }

        public static string SerializeToPayload(this ChannelList channelList)
        {
            return
                "{\"channels\":[" + string.Join(",", channelList.Channels.Select(x => x.SerializeToPayload())) +
                "]}";
        }
    }
}