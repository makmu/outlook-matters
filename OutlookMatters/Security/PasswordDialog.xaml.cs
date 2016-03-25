using System.Windows;
using OutlookMatters.Utils;

namespace OutlookMatters.Security
{
    /// <summary>
    ///     Interaction logic for PasswordDialog.xaml
    /// </summary>
    public partial class PasswordDialog : IPasswordProvider
    {
        public PasswordDialog()
        {
            InitializeComponent();
        }

        public string GetPassword(string username)
        {
            Username.Content = username;
            ShowDialog();
            if (DialogResult == true)
            {
                return Password.Password;
            }
            throw new UserAbortException("cannot provide password for '" + username + "': user abort");
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}