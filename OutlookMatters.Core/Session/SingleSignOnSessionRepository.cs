using System.Threading.Tasks;
using OutlookMatters.Core.Cache;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Session
{
    public class SingleSignOnSessionRepository : ICache, ISessionRepository
    {
        private readonly IClient _client;
        private readonly IPasswordProvider _passwordProvider;
        private readonly ISettingsLoadService _settingsLoadService;

        private ISession _session;

        public SingleSignOnSessionRepository(IClient client, ISettingsLoadService settingsLoadService,
            IPasswordProvider passwordProvider)
        {
            _client = client;
            _settingsLoadService = settingsLoadService;
            _passwordProvider = passwordProvider;
        }

        public void Invalidate()
        {
            _session = null;
        }

        public async Task<ISession> RestoreSession()
        {
            if (_session == null)
            {
                var settings = _settingsLoadService.Load();
                var password = _passwordProvider.GetPassword(settings.Username);
                _session = await Task.Run(() =>
                    _client.LoginByUsername(
                        settings.MattermostUrl,
                        settings.TeamId,
                        settings.Username,
                        password));
            }
            return _session;
        }
    }
}