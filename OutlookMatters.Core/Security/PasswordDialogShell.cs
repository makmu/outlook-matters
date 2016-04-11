using OutlookMatters.Core.Utils;

namespace OutlookMatters.Core.Security
{
    public class PasswordDialogShell : IPasswordProvider
    {
        public string GetPassword(string username)
        {
            var dialog = new PasswordDialog();
            dialog.Username.Content = username;
            dialog.ShowDialog();
            if (dialog.DialogResult == true)
            {
                return dialog.Password.Password;
            }
            throw new UserAbortException("cannot provide password for '" + username + "': user abort");
        }
    }
}