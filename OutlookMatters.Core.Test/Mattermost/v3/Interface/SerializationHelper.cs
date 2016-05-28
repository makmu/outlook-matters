using OutlookMatters.Core.Mattermost.v3.Interface;

namespace Test.OutlookMatters.Core.Mattermost.v3.Interface
{
    public static class SerializationHelper
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
    }
}