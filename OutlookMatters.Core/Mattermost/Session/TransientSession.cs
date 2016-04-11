using OutlookMatters.Core.Mattermost.DataObjects;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Mattermost.Session
{
    public class TransientSession : ISession, ICache
    {
        private readonly IMattermost _mattermost;
        private readonly IPasswordProvider _passwordProvider;
        private readonly ISettingsLoadService _settingsLoadService;

        private ISession _session;

        public TransientSession(IMattermost mattermost, ISettingsLoadService settingsLoadService,
            IPasswordProvider passwordProvider)
        {
            _mattermost = mattermost;
            _settingsLoadService = settingsLoadService;
            _passwordProvider = passwordProvider;
        }

        private ISession Session
        {
            get
            {
                if (_session == null)
                {
                    var settings = _settingsLoadService.Load();
                    var password = _passwordProvider.GetPassword(settings.Username);
                    _session = _mattermost.LoginByUsername(
                        settings.MattermostUrl,
                        settings.TeamId,
                        settings.Username,
                        password);
                    _session.FetchChannelList();
                }
                return _session;
            }
        }

        public void CreatePost(string channelId, string message, string rootId = "")
        {
            Session.CreatePost(channelId, message, rootId);
        }

        public Post GetRootPost(string postId)
        {
            return Session.GetRootPost(postId);
        }

        public ChannelList FetchChannelList()
        {
            return Session.FetchChannelList();
        }

        public void Invalidate()
        {
            _session = null;
        }
    }
}