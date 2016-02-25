using OutlookMatters.Properties;

namespace OutlookMatters
{
    public class ApplicationSettingsProvider: ISettingsProvider
    {
        public string ChannelId
        {
            get { return Settings.Default.ChannelId; }
            set { Settings.Default.ChannelId = value; }
        }

        public string Url
        {
            get { return Settings.Default.MattermostUrl; }
            set { Settings.Default.MattermostUrl = value; }
        }

        public string TeamId
        {
            get { return Settings.Default.TeamId; }
            set { Settings.Default.TeamId = value; }
        }

        public string Username
        {
            get { return Settings.Default.Email; }
            set { Settings.Default.Email = value; }
        }

        public string Password
        {
            get { return Settings.Default.Password; }
            set { Settings.Default.Password = value; }
        }
    }
}
