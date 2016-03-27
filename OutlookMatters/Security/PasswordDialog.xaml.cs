using System.Windows;
using OutlookMatters.Utils;

namespace OutlookMatters.Security
{
    /// <summary>
    ///     Interaction logic for PasswordDialog.xaml
    /// </summary>
    public partial class PasswordDialog
    {
        public PasswordDialog()
        {
            InitializeComponent();
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