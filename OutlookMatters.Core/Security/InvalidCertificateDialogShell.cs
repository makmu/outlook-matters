using System.Windows;

namespace OutlookMatters.Core.Security
{
    public class InvalidCertificateDialogShell: ITrustInvalidSslQuestion
    {
        private readonly Window _dispatcher;

        public InvalidCertificateDialogShell()
        {
            _dispatcher = new Window();
        }

        public bool GetAnswer(string url, string message)
        {
            bool result = false;
            _dispatcher.Dispatcher.Invoke(() =>
            {
                var dialog = new InvalidCertificateDialog {Url = {Content = url}, Message = {Text = message}};
                dialog.ShowDialog();
                result = dialog.DialogResult ?? true;
            });
            return result;
        }
    }
}
