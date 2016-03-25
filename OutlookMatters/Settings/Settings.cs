namespace OutlookMatters.Settings
{
    public class Settings
    {
        public Settings(string mattermostUrl, string teamId, string channelId, string username)
        {
            MattermostUrl = mattermostUrl;
            TeamId = teamId;
            ChannelId = channelId;
            Username = username;
        }

        public string MattermostUrl { get; }
        public string TeamId { get; }
        public string ChannelId { get; }
        public string Username { get; }

        protected bool Equals(Settings other)
        {
            return string.Equals(MattermostUrl, other.MattermostUrl) && string.Equals(TeamId, other.TeamId) &&
                   string.Equals(ChannelId, other.ChannelId) && string.Equals(Username, other.Username);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Settings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MattermostUrl != null ? MattermostUrl.GetHashCode() : 0;
                hashCode = (hashCode*397) ^ (TeamId != null ? TeamId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ChannelId != null ? ChannelId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Username != null ? Username.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}