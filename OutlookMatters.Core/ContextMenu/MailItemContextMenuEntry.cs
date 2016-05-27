using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Error;
using OutlookMatters.Core.Mail;
using OutlookMatters.Core.Mattermost.Interface;
using OutlookMatters.Core.Reply;
using OutlookMatters.Core.Session;
using OutlookMatters.Core.Settings;
using OutlookMatters.Core.Utils;
using Office = Microsoft.Office.Core;

namespace OutlookMatters.Core.ContextMenu
{
    [ComVisible(true)]
    public class MailItemContextMenuEntry : Office.IRibbonExtensibility
    {
        private const string CHANNEL_BUTTON_ID_PREFIX = "channel_id-";

        private readonly IErrorDisplay _errorDisplay;
        private readonly IMailExplorer _explorer;
        private readonly IStringProvider _rootPostIdProvider;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISettingsLoadService _settingsLoadService;
        private readonly ISettingsSaveService _settingsSaveService;
        private readonly ISettingsUserInterface _settingsUi;


        public MailItemContextMenuEntry(IMailExplorer explorer,
            ISettingsLoadService settingsLoadService,
            ISettingsSaveService settingsSaveService,
            IErrorDisplay errorDisplay,
            ISettingsUserInterface settingsUi,
            ISessionRepository sessionRepository,
            IStringProvider rootPostIdProvider)
        {
            _explorer = explorer;
            _settingsLoadService = settingsLoadService;
            _settingsSaveService = settingsSaveService;
            _errorDisplay = errorDisplay;
            _settingsUi = settingsUi;
            _sessionRepository = sessionRepository;
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
            var channelList = JsonConvert.DeserializeObject<ChannelListSetting>(settings.ChannelsMap);
            if (channelList != null)
            {
                for (int index = 0; index < channelList.Channels.Count; index++)
                {
                    if (channelList.Channels[index].Type == ChannelTypeSetting.Public)
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

        public async Task OnPostIntoChannelClick(Office.IRibbonControl control)
        {
            var channelId = control.Id.Substring(CHANNEL_BUTTON_ID_PREFIX.Length);
            var message = FormatMessage();
            try
            {
                var session = await _sessionRepository.RestoreSession();
                var channel = session.GetChannel(channelId);
                await Task.Run(() => channel.CreatePost(message));
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

        public async Task OnRefreshChannelListClick(Office.IRibbonControl control)
        {
            try
            {
                var session = await _sessionRepository.RestoreSession();
                var channelList = await Task.Run(() => session.GetChannels());
                var channelSettings = new ChannelListSetting
                {
                    Channels = channelList.Select(x => x.ToSetting()).ToList()
                };
                var channelMap = JsonConvert.SerializeObject(channelSettings);
                await Task.Run(() => _settingsSaveService.SaveChannels(channelMap));
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

        public async Task OnReplyClick(Office.IRibbonControl control)
        {
            var message = FormatMessage();
            try
            {
                var postId = _rootPostIdProvider.Get();
                var session = await _sessionRepository.RestoreSession();
                var post = await Task.Run(() => session.GetPost(postId));
                await Task.Run(() => post.Reply(message));
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
            var mail = _explorer.QuerySelectedMailItem();
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