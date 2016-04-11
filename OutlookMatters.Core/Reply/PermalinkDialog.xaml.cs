using System.Windows;

namespace OutlookMatters.Core.Reply
{
    /// <summary>
    ///     Interaction logic for PermalinkDialog.xaml
    /// </summary>
    public partial class PermalinkDialog
    {
        public PermalinkDialog()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}