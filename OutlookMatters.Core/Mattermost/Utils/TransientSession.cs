using OutlookMatters.Core.Cache;
using OutlookMatters.Core.Mattermost.Interface;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Mattermost.Utils
{
    public class TransientSession : ISession, ICache
    {
        private readonly IClient _client;
        private readonly IPasswordProvider _passwordProvider;
        private readonly ISettingsLoadService _settingsLoadService;

        private ISession _session;

        public TransientSession(IClient client, ISettingsLoadService settingsLoadService,
            IPasswordProvider passwordProvider)
        {
            _client = client;
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
                    _session = _client.LoginByUsername(
                        settings.MattermostUrl,
                        settings.TeamId,
                        settings.Username,
                        password);
                    _session.FetchChannelList();
                }
                return _session;
            }
        }

        public Payload CreatePost(string channelId, string message, string rootId = "")
        {
            return Session.CreatePost(channelId, message, rootId);
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