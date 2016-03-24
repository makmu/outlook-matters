using OutlookMatters.Error;
using OutlookMatters.Mail;
using OutlookMatters.Mattermost.Session;
using OutlookMatters.Settings;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.Core;

namespace OutlookMatters.ContextMenu
{
    [ComVisible(true)]
    public class MailItemContextMenuEntry : Office.IRibbonExtensibility
    {
        private readonly IMailExplorer _explorer;
        private readonly ISettingsLoadService _settingsLoadService;
        private readonly IErrorDisplay _errorDisplay;
        private readonly ISettingsUserInterface _settingsUi;
        private readonly ISessionCache _sessionCache;

     
        public MailItemContextMenuEntry(IMailExplorer explorer, ISettingsLoadService settingsLoadService, IErrorDisplay errorDisplay, ISettingsUserInterface settingsUi, ISessionCache sessionCache)
        {
            _explorer = explorer;
            _settingsLoadService = settingsLoadService;
            _errorDisplay = errorDisplay;
            _settingsUi = settingsUi;
            _sessionCache = sessionCache;
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
                _sessionCache.Session?.CreatePost(channelId, message);
            }
            catch (Exception exception)
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