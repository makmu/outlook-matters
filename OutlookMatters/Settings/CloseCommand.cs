using System;
using System.Windows.Input;

namespace OutlookMatters.Settings
{
    public class CloseCommand : ICommand
    {
        private readonly IClosableWindow _window;

        public CloseCommand(IClosableWindow window)
        {
            _window = window;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _window.Close();
        }

        public event EventHandler CanExecuteChanged;
    }
}