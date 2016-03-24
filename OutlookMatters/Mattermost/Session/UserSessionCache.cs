using OutlookMatters.Security;
using OutlookMatters.Settings;
using System;

namespace OutlookMatters.Mattermost.Session
{
    public class UserSessionCache : ISessionCache
    {
        private readonly IMattermost _mattermost;
        private readonly ISettingsLoadService _settingsLoadService;
        private readonly IPasswordProvider _passwordProvider;

        public UserSessionCache(IMattermost mattermost, ISettingsLoadService settingsLoadService, IPasswordProvider passwordProvider)
        {
            _mattermost = mattermost;
            _settingsLoadService = settingsLoadService;
            _passwordProvider = passwordProvider;
        }

        private ISession _session;
        private DateTime? lastChanged;

        public ISession Session
        {
            get
            {
                if (_session == null || lastChanged == null || lastChanged < _settingsLoadService.LastChanged)
                {
                    lastChanged = _settingsLoadService.LastChanged;

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
    }
}
