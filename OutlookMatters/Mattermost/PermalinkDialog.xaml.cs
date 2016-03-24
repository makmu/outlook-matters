using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OutlookMatters.Mattermost;

namespace OutlookMatters.Security
{
    /// <summary>
    /// Interaction logic for PermalinkDialog.xaml
    /// </summary>
    public partial class PermalinkDialog: IStringProvider
    {
        public PermalinkDialog()
        {
            InitializeComponent();
        }

        public string Get()
        {
            ShowDialog();
            if (DialogResult == true)
            {
                return Permalink.Text;
            }
            throw new Exception("cannot provide post id for: user abort");
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
