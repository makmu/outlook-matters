using System;

namespace OutlookMatters.Settings
{
    public class ApplicationSettingsService: ISettingsLoadService, ISettingsSaveService
    {
        public DateTime LastChanged { get; private set; } = DateTime.Now;

        public Settings Load()
        {
            return new Settings(
                Properties.Settings.Default.MattermostUrl,
                Properties.Settings.Default.TeamId,
                Properties.Settings.Default.ChannelId,
                Properties.Settings.Default.Username );
        }

        public void Save(Settings settings)
        {
            Properties.Settings.Default.MattermostUrl = settings.MattermostUrl;
            Properties.Settings.Default.TeamId = settings.TeamId;
            Properties.Settings.Default.ChannelId = settings.ChannelId;
            Properties.Settings.Default.Username = settings.Username;
            Properties.Settings.Default.Save();

            LastChanged = DateTime.Now;
        }

       
    }
}
