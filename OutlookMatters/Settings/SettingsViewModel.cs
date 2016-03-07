using System.Windows.Input;

namespace OutlookMatters.Settings
{
    public class SettingsViewModel
    {
        public SettingsViewModel(Settings settings, ICommand saveCommand, ICommand cancelCommand )
        {
            Save = saveCommand;
            Cancel = cancelCommand;
            MattermostUrl = settings.MattermostUrl;
            TeamId = settings.TeamId;
            ChannelId = settings.ChannelId;
            Username = settings.Username;
        }

        public string MattermostUrl { get; set; }
        public string TeamId { get; set; }
        public string ChannelId { get; set; }
        public string Username { get; set; }
        public ICommand Save { get; }
        public ICommand Cancel { get; }
    }
}