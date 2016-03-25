using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using OutlookMatters.Error;
using OutlookMatters.Mail;
using OutlookMatters.Mattermost;
using OutlookMatters.Mattermost.Session;
using OutlookMatters.Settings;
using OutlookMatters.Utils;
using Office = Microsoft.Office.Core;

namespace OutlookMatters.ContextMenu
{
    [ComVisible(true)]
    public class MailItemContextMenuEntry : Office.IRibbonExtensibility
    {
        private readonly IErrorDisplay _errorDisplay;
        private readonly IMailExplorer _explorer;
        private readonly IStringProvider _rootPostIdProvider;
        private readonly ISession _session;
        private readonly ISettingsLoadService _settingsLoadService;
        private readonly ISettingsUserInterface _settingsUi;


        public MailItemContextMenuEntry(IMailExplorer explorer,
            ISettingsLoadService settingsLoadService,
            IErrorDisplay errorDisplay,
            ISettingsUserInterface settingsUi,
            ISession session,
            IStringProvider rootPostIdProvider)
        {
            _explorer = explorer;
            _settingsLoadService = settingsLoadService;
            _errorDisplay = errorDisplay;
            _settingsUi = settingsUi;
            _session = session;
            _rootPostIdProvider = rootPostIdProvider;
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
            xmlString += @"  <splitButton id=""PostSplitButton"">";
            xmlString += @"    <button id=""PostButton"" label=""Post"" onAction=""OnPostClick""/>";
            xmlString += @"    <menu id=""menu"">";
            xmlString += @"      <button id=""QuickPostButton"" label=""Quick Post"" onAction=""OnPostClick""/>";
            xmlString += @"      <button id=""ReplyButton"" label=""As Reply..."" onAction=""OnReplyClick""/>";
            xmlString += @"    </menu>";
            xmlString += @"  </splitButton>";
            xmlString += @"  <menuSeparator id=""separator""/>";
            xmlString +=
                @"  <button id=""SettingsButton"" imageMso=""ComAddInsDialog"" label=""Settings..."" onAction=""OnSettingsClick"" />";
            xmlString += @"</menu>";
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
                _session.CreatePost(channelId, message);
            }
            catch (Exception exception)
            {
                _errorDisplay.Display(exception);
            }
        }

        public void OnReplyClick(Office.IRibbonControl control)
        {
            var settings = _settingsLoadService.Load();
            var channelId = settings.ChannelId;
            var mail = _explorer.QuerySelectedMailData();
            var message = ":email: From: " + mail.SenderName + "\n";
            message += ":email: Subject: " + mail.Subject + "\n";
            message += mail.Body;
            try
            {
                var rootId = _rootPostIdProvider.Get();
                _session.CreatePost(channelId, message, rootId);
            }
            catch (UserAbortException)
            {
            }
            catch (Exception exception)
            {
                _errorDisplay.Display(exception);
            }
        }

        private static string GetResourceText(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resourceNames = asm.GetManifestResourceNames();
            foreach (var name in resourceNames)
            {
                if (string.Compare(resourceName, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (var manifestStream = asm.GetManifestResourceStream(name))
                    {
                        if (manifestStream == null)
                        {
                            return null;
                        }
                        using (var resourceReader = new StreamReader(manifestStream))
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }
    }
}