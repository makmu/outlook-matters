using OutlookMatters.Mattermost.DataObjects;
using OutlookMatters.Security;
using OutlookMatters.Settings;

namespace OutlookMatters.Mattermost.Session
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
                }
                return _session;
            }
        }

        public void CreatePost(string channelId, string message, string rootId = "")
        {
            Session.CreatePost(channelId, message, rootId);
        }

        public Post GetPostById(string postId)
        {
            return Session.GetPostById(postId);
        }

        public void Invalidate()
        {
            _session = null;
        }
    }
}