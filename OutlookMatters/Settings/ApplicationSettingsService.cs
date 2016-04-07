using OutlookMatters.Mattermost.Session;

namespace OutlookMatters.Settings
{
    public class ApplicationSettingsService : ISettingsLoadService, ISettingsSaveService
    {
        private readonly ICache _cache;

        public ApplicationSettingsService(ICache cache)
        {
            _cache = cache;
        }

        public Settings Load()
        {
            return new Settings(
                Properties.Settings.Default.MattermostUrl,
                Properties.Settings.Default.TeamId,
                Properties.Settings.Default.ChannelId,
                Properties.Settings.Default.Username,
                Properties.Settings.Default.ChannelsMap);
        }

        public void Save(Settings settings)
        {
            Properties.Settings.Default.MattermostUrl = settings.MattermostUrl;
            Properties.Settings.Default.TeamId = settings.TeamId;
            Properties.Settings.Default.ChannelId = settings.ChannelId;
            Properties.Settings.Default.Username = settings.Username;
            Properties.Settings.Default.ChannelsMap = settings.ChannelsMap;
            Properties.Settings.Default.Save();
            _cache.Invalidate();
        }
    }
}