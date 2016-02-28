using System;
using System.Windows;

namespace OutlookMatters.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.MattermostUrl = url.Text;
            Properties.Settings.Default.ChannelId = channel.Text;
            Properties.Settings.Default.Password = password.Password;
            Properties.Settings.Default.Email = email.Text;
            Properties.Settings.Default.TeamId = teamId.Text;
            Properties.Settings.Default.Save();
            Close();
        }

        private void OnWindowLoaded(object sender, EventArgs e)
        {
            url.Text = Properties.Settings.Default.MattermostUrl;
            channel.Text = Properties.Settings.Default.ChannelId;
            email.Text = Properties.Settings.Default.Email;
            teamId.Text = Properties.Settings.Default.TeamId;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
