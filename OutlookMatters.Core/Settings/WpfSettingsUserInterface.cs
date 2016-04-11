namespace OutlookMatters.Core.Settings
{
    public class WpfSettingsUserInterface : ISettingsUserInterface
    {
        private readonly ISettingsLoadService _loadService;
        private readonly ISettingsSaveService _saveService;

        public WpfSettingsUserInterface(ISettingsLoadService loadService, ISettingsSaveService saveService)
        {
            _loadService = loadService;
            _saveService = saveService;
        }

        public void OpenSettings()
        {
            var settings = _loadService.Load();
            var window = new SettingsWindow();
            window.DataContext = new SettingsViewModel(settings, new SaveCommand(_saveService, window),
                new CloseCommand(window));
            window.ShowDialog();
        }
    }
}