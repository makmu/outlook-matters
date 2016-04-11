namespace OutlookMatters.Core.Settings
{
    public class AddInSettings
    {
        public AddInSettings(string mattermostUrl, string teamId, string username, string channelsMap)
        {
            MattermostUrl = mattermostUrl;
            TeamId = teamId;
            Username = username;
            ChannelsMap = channelsMap;
        }

        public string MattermostUrl { get; private set; }
        public string TeamId { get; private set; }
        public string Username { get; private set; }
        public string ChannelsMap { get; private set; }

        protected bool Equals(AddInSettings other)
        {
            return string.Equals(MattermostUrl, other.MattermostUrl) && string.Equals(TeamId, other.TeamId) && 
                   string.Equals(Username, other.Username) &&
                   string.Equals(ChannelsMap, other.ChannelsMap);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AddInSettings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MattermostUrl != null ? MattermostUrl.GetHashCode() : 0;
                hashCode = (hashCode*397) ^ (TeamId != null ? TeamId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ChannelsMap != null ? ChannelsMap.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}