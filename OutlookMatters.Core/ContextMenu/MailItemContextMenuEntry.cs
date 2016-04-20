using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using OutlookMatters.Core.Error;
using OutlookMatters.Core.Mail;
using OutlookMatters.Core.Mattermost.Interface;
using OutlookMatters.Core.Reply;
using OutlookMatters.Core.Settings;
using OutlookMatters.Core.Utils;
using Office = Microsoft.Office.Core;

namespace OutlookMatters.Core.ContextMenu
{
    [ComVisible(true)]
    public class MailItemContextMenuEntry : Office.IRibbonExtensibility
    {
        private const string CHANNEL_BUTTON_ID_PREFIX = "channel_id-";
        private const int MAX_MESSAGE_LENGTH = 4000;

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
            if (ribbonId != "Microsoft.Outlook.Explorer")
            {
                return null;
            }
            return GetResourceText("OutlookMatters.Core.ContextMenu.MailItemContextMenuEntry.xml");
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
                    if (channelList.Channels[index].Type == ChannelType.Public)
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
            var messageParts = FormatMessage();
            try
            {
                var rootId = "";
                foreach (var messagePart in messageParts)
                {
                    var payload = _session.CreatePost(channelId, messagePart, rootId);
                    if (rootId == string.Empty)
                    {
                        rootId = payload.PostId;
                    }
                }
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
            try
            {
                var channelList = _session.FetchChannelList();
                var channelMap = JsonConvert.SerializeObject(channelList);
                _settingsSaveService.SaveChannels(channelMap);
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

        public void OnReplyClick(Office.IRibbonControl control)
        {
            var messageParts = FormatMessage();
            try
            {
                var postId = _rootPostIdProvider.Get();
                var rootPost = _session.GetRootPost(postId);
                var rootId = rootPost.id;

                _session.CreatePost(rootPost.channel_id, messageParts[0], rootId);
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

        private List<string> FormatMessage()
        {
            var mail = _explorer.QuerySelectedMailItem();
            var message = ":email: From: " + mail.SenderName + "\n";
            message += ":email: Subject: " + mail.Subject + "\n";
            message += mail.Body;

            return GenerateCompleteMessage(message);
        }

        private List<string> GenerateCompleteMessage(string message)
        {
            var messageParts = new List<string>();
            
            while (message.Length > 0)
            {
                if (message.Length > MAX_MESSAGE_LENGTH)
                {
                    var messageSlice = message.Substring(0, MAX_MESSAGE_LENGTH);
                    var positionOfLastSpace = messageSlice.LastIndexOf(" ");
                    var slicedMessagePart = message.Substring(0, positionOfLastSpace);
                    messageParts.Add(slicedMessagePart);
                    var leftOverLength = message.Length - positionOfLastSpace;
                    message = message.Substring(positionOfLastSpace, leftOverLength);
                }
                else
                {
                    messageParts.Add(message);
                    break;
                }
            }
            return messageParts;
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