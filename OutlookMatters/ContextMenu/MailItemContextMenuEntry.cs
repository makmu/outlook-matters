﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
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
        private const string CHANNEL_BUTTON_ID_PREFIX = "channel_id-";
        private const string SUBSCRIBED_CHANNEL_TYPE = "O";

        private readonly IErrorDisplay _errorDisplay;
        private readonly IMailExplorer _explorer;
        private readonly IStringProvider _rootPostIdProvider;
        private readonly ISession _session;
        private readonly ISettingsLoadService _settingsLoadService;
        private readonly ISettingsSaveService _settingsSaveService;
        private readonly ISettingsUserInterface _settingsUi;


        public MailItemContextMenuEntry(IMailExplorer explorer,
            ISettingsLoadService settingsLoadService,
            ISettingsSaveService settingsSaveService,
            IErrorDisplay errorDisplay,
            ISettingsUserInterface settingsUi,
            ISession session,
            IStringProvider rootPostIdProvider)
        {
            _explorer = explorer;
            _settingsLoadService = settingsLoadService;
            _settingsSaveService = settingsSaveService;
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
            var settings = _settingsLoadService.Load();
            var channelList = JsonConvert.DeserializeObject<ChannelList>(settings.ChannelsMap);
            if (channelList != null)
            {
                for (int index = 0; index < channelList.Channels.Count; index++)
                {
                    if (channelList.Channels[index].Type == SUBSCRIBED_CHANNEL_TYPE)
                    {
                        xmlString += CreateChannelButton(channelList.Channels[index].ChannelId,
                            channelList.Channels[index].ChannelName);
                    }
                }
            }

            xmlString += @"  <menuSeparator id=""specialSectionSeparator""/>";
            xmlString +=
                @"  <button id=""ReplyButton"" label=""As Reply..."" onAction=""OnReplyClick"" imageMso=""Reply"" />";
            xmlString += @"  <menuSeparator id=""settingsSectionSeparator""/>";
            xmlString +=
                @"  <button id=""SettingsButton"" imageMso=""ComAddInsDialog"" label=""Settings..."" onAction=""OnSettingsClick"" />";
            xmlString +=
                @"  <button id=""GetChannels"" label=""Refresh Channels"" onAction=""OnRefreshChannelListClick"" imageMso=""AccessRefreshAllLists"" />";
            xmlString += @"</menu>";
            return xmlString;
        }


        private string CreateChannelButton(string buttonId, string channelName)
        {
            var button = @"<button id=""" + CHANNEL_BUTTON_ID_PREFIX + buttonId + @""" label=""" + channelName +
                         @""" onAction=""OnPostIntoChannelClick"" imageMso=""Forward"" />";
            return button;
        }

        public void OnSettingsClick(Office.IRibbonControl control)
        {
            _settingsUi.OpenSettings();
        }

        public void OnPostIntoChannelClick(Office.IRibbonControl control)
        {
            var channelId = control.Id.Substring(CHANNEL_BUTTON_ID_PREFIX.Length);
            var message = FormatMessage();
            try
            {
                _session.CreatePost(channelId, message);
            }
            catch (MattermostException mex)
            {
                _errorDisplay.Display(mex);
            }
            catch (Exception exception)
            {
                _errorDisplay.Display(exception);
            }
        }
        
        public void OnRefreshChannelListClick(Office.IRibbonControl control)
        {
            var channelList = _session.FetchChannelList();
            var channelMap = JsonConvert.SerializeObject(channelList);
            var oldSettings = _settingsLoadService.Load();
            var settings = new Settings.Settings(oldSettings.MattermostUrl, oldSettings.TeamId, oldSettings.ChannelId,
                oldSettings.Username, channelMap);
            _settingsSaveService.Save(settings);
        }

        public void OnReplyClick(Office.IRibbonControl control)
        {
            var settings = _settingsLoadService.Load();
            var channelId = settings.ChannelId;
            var message = FormatMessage();
            try
            {
                var rootId = _rootPostIdProvider.Get();
                _session.CreatePost(channelId, message, rootId);
            }
            catch (UserAbortException)
            {
            }
            catch (MattermostException mex)
            {
                _errorDisplay.Display(mex);
            }
            catch (Exception exception)
            {
                _errorDisplay.Display(exception);
            }
        }

        private string FormatMessage()
        {
            var mail = _explorer.QuerySelectedMailData();
            var message = ":email: From: " + mail.SenderName + "\n";
            message += ":email: Subject: " + mail.Subject + "\n";
            message += mail.Body;
            return message;
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