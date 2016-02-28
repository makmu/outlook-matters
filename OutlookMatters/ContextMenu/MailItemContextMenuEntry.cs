using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using OutlookMatters.Mail;
using OutlookMatters.Mattermost;
using OutlookMatters.Security;
using OutlookMatters.Settings;
using Office = Microsoft.Office.Core;

namespace OutlookMatters.ContextMenu
{
    [ComVisible(true)]
    public class MailItemContextMenuEntry : Office.IRibbonExtensibility
    {
        private readonly IMailExplorer _explorer;
        private readonly IMattermost _mattermost;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IPasswordProvider _passwordProvider;

        private ISession _session;
        private ISession Session
        {
            get
            {
                if (_session == null)
                {
                    var password = _passwordProvider.GetPassword(_settingsProvider.Username);
                    _session = _mattermost.LoginByUsername(
                        _settingsProvider.Url,
                        _settingsProvider.TeamId,
                        _settingsProvider.Username,
                        password);
                }
                return _session;
            }
        }

        public MailItemContextMenuEntry(IMailExplorer explorer, IMattermost mattermost, ISettingsProvider settingsProvider, IPasswordProvider passwordProvider)
        {
            _explorer = explorer;
            _mattermost = mattermost;
            _settingsProvider = settingsProvider;
            _passwordProvider = passwordProvider;
        }

        public string GetCustomUI(string ribbonId)
        {
            switch (ribbonId)
            {
                case "Microsoft.Outlook.Explorer":
                    return GetResourceText("OutlookMatters.ContextMenu.MailItemContextMenuEntry.xml");
                default:
                    return string.Empty;
            }
        }

        public void OnSettingsClick(Office.IRibbonControl control)
        {
            var window = new SettingsWindow();
            window.ShowDialog();
        }

        public void OnPostClick(Office.IRibbonControl control)
        {
            var channelId = _settingsProvider.ChannelId;
            var mailbody = _explorer.GetSelectedMailBody();

            Session.CreatePost(channelId, mailbody);
        }
        
        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (var resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}