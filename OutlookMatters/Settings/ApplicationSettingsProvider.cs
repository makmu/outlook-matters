namespace OutlookMatters.Settings
{
    public class ApplicationSettingsProvider: ISettingsProvider
    {
        public string ChannelId
        {
            get { return Properties.Settings.Default.ChannelId; }
            set { Properties.Settings.Default.ChannelId = value; }
        }

        public string Url
        {
            get { return Properties.Settings.Default.MattermostUrl; }
            set { Properties.Settings.Default.MattermostUrl = value; }
        }

        public string TeamId
        {
            get { return Properties.Settings.Default.TeamId; }
            set { Properties.Settings.Default.TeamId = value; }
        }

        public string Username
        {
            get { return Properties.Settings.Default.Username; }
            set { Properties.Settings.Default.Username = value; }
        }
    }
}
