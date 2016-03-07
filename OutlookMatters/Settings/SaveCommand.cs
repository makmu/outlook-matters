using System;
using System.Windows.Input;

namespace OutlookMatters.Settings
{
    public class SaveCommand: ICommand
    {
        private readonly ISettingsSaveService _saveService;
        private readonly IClosableWindow _window;

        public SaveCommand(ISettingsSaveService saveService, IClosableWindow window)
        {
            _saveService = saveService;
            _window = window;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = parameter as SettingsViewModel;
            if (viewModel == null) return;
            var settings = new Settings(
                viewModel.MattermostUrl,
                viewModel.TeamId,
                viewModel.ChannelId,
                viewModel.Username);
            _saveService.Save(settings);
            _window.Close();
        }

        public event EventHandler CanExecuteChanged;
    }
}
