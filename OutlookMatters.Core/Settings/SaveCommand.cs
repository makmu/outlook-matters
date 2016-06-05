using System;
using System.Windows.Input;

namespace OutlookMatters.Core.Settings
{
    public class SaveCommand : ICommand
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
            if (viewModel == null)
            {
                throw new ArgumentException(@"Invalid ViewModel ", "parameter");
            }
            _saveService.SaveCredentials(viewModel.MattermostUrl, viewModel.TeamId, viewModel.Username, viewModel.Version);
            _saveService.SaveChannels(string.Empty);
            _window.Close();
        }

        public event EventHandler CanExecuteChanged;
    }
}