using System.Windows.Input;

namespace OutlookMatters.Core.Settings
{
    public class SettingsViewModel
    {
        public SettingsViewModel(AddInSettings settings, ICommand saveCommand, ICommand cancelCommand)
        {
            Save = saveCommand;
            Cancel = cancelCommand;
            MattermostUrl = settings.MattermostUrl;
            TeamId = settings.TeamId;
            Username = settings.Username;
            Version = settings.Version;
        }

        public string MattermostUrl { get; set; }
        public string TeamId { get; set; }
        public string Username { get; set; }
        public ICommand Save { get; private set; }
        public ICommand Cancel { get; private set; }
        public MattermostVersion Version { get; set; }
    }
}