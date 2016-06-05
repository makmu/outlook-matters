using System.Threading.Tasks;
using OutlookMatters.Core.Cache;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Session
{
    public class SingleSignOnSessionRepository : ICache, ISessionRepository
    {
        private readonly IPasswordProvider _passwordProvider;
        private readonly IClientFactory _clientFactory;
        private readonly ISettingsLoadService _settingsLoadService;

        private ISession _session;

        public SingleSignOnSessionRepository(IClientFactory clientFactory, ISettingsLoadService settingsLoadService,
            IPasswordProvider passwordProvider)
        {
            _clientFactory = clientFactory;
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
                var client = _clientFactory.GetClient(settings.Version);
                _session = await Task.Run(() =>
                    client.LoginByUsername(
                        settings.MattermostUrl,
                        settings.TeamId,
                        settings.Username,
                        password));
            }
            return _session;
        }
    }
}