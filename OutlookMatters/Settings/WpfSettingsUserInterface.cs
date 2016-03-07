namespace OutlookMatters.Settings
{
    class WpfSettingsUserInterface: ISettingsUserInterface
    {
        public void OpenSettings()
        {
            var window = new SettingsWindow();
            window.ShowDialog();
        }
    }
}
