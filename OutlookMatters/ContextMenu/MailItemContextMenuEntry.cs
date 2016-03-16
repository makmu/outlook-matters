using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using OutlookMatters.Error;
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
        private readonly ISettingsLoadService _settingsLoadService;
        private readonly IPasswordProvider _passwordProvider;
        private readonly IErrorDisplay _errorDisplay;
        private readonly ISettingsUserInterface _settingsUi;

        private ISession _session;
        private ISession Session
        {
            get
            {
                if (_session == null)
                {
                    try
                    {
                        var settings = _settingsLoadService.Load();
                        var password = _passwordProvider.GetPassword(settings.Username);
                        _session = _mattermost.LoginByUsername(
                            settings.MattermostUrl,
                            settings.TeamId,
                            settings.Username,
                            password);
                    }
                    catch (WebException exception)
                    {
                        _errorDisplay.Display(exception);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                return _session;
            }
        }

        public MailItemContextMenuEntry(IMailExplorer explorer, IMattermost mattermost, ISettingsLoadService settingsLoadService, IPasswordProvider passwordProvider, IErrorDisplay errorDisplay, ISettingsUserInterface settingsUi)
        {
            _explorer = explorer;
            _mattermost = mattermost;
            _settingsLoadService = settingsLoadService;
            _passwordProvider = passwordProvider;
            _errorDisplay = errorDisplay;
            _settingsUi = settingsUi;
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

        public string GetDynamicMenu(Office.IRibbonControl control)
        {
            var xmlString = @"<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">";
            xmlString += @"<button id=""PostButton"" label=""Post"" onAction=""OnPostClick"" />";
            xmlString += @"<menuSeparator id=""separator""/>";
            xmlString += @"<button id=""SettingsButton"" imageMso=""ComAddInsDialog"" label=""Settings..."" onAction=""OnSettingsClick"" />";
            xmlString += "</menu>";
            return xmlString;
        }

        public void OnSettingsClick(Office.IRibbonControl control)
        {
            _settingsUi.OpenSettings();
        }

        public void OnPostClick(Office.IRibbonControl control)
        {
            var settings = _settingsLoadService.Load();
            var channelId = settings.ChannelId;
            var mail = _explorer.QuerySelectedMailData();
            var message = ":email: From: " + mail.SenderName + "\n";
            message += ":email: Subject: " + mail.Subject + "\n";
            message += mail.Body;

            try
            {
                Session?.CreatePost(channelId, message);
            }
            catch (WebException exception)
            {
                _errorDisplay.Display(exception);
            }
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