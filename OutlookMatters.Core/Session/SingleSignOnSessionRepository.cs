using System.Net;
using System.Threading.Tasks;
using OutlookMatters.Core.Cache;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Security;
using OutlookMatters.Core.Settings;
using OutlookMatters.Core.Utils;

namespace OutlookMatters.Core.Session
{
    public class SingleSignOnSessionRepository : ICache, ISessionRepository
    {
        private readonly IPasswordProvider _passwordProvider;
        private readonly ITrustInvalidSslQuestion _invalidSslQuestion;
        private readonly IClientFactory _clientFactory;
        private readonly ISettingsLoadService _settingsLoadService;

        private ISession _session;
        private readonly IServerCertificateValidator _serverCertificateValidator;

        public SingleSignOnSessionRepository(
            IClientFactory clientFactory,
            ISettingsLoadService settingsLoadService,
            IPasswordProvider passwordProvider,
            ITrustInvalidSslQuestion invalidSslQuestion,
            IServerCertificateValidator serverCertificateValidator)
        {
            _clientFactory = clientFactory;
            _settingsLoadService = settingsLoadService;
            _passwordProvider = passwordProvider;
            _invalidSslQuestion = invalidSslQuestion;
            _serverCertificateValidator = serverCertificateValidator;
        }

        public void Invalidate()
        {
            _session = null;
            _serverCertificateValidator.EnableValidation();
        }

        public async Task<ISession> RestoreSession()
        {
            if (_session == null)
            {
                var settings = _settingsLoadService.Load();
                var password = _passwordProvider.GetPassword(settings.Username);
                var client = _clientFactory.GetClient(settings.Version);
                bool retryLogin;
                do
                {
                    try
                    {
                        _session = await Task.Run(() =>
                            client.LoginByUsername(
                                settings.MattermostUrl,
                                settings.TeamId,
                                settings.Username,
                                password));
                        retryLogin = false;
                    }
                    catch (WebException wex)
                    {
                        if (wex.Status == WebExceptionStatus.TrustFailure &&
                            _invalidSslQuestion.GetAnswer(settings.MattermostUrl, wex.InnerException.Message))
                        {
                            retryLogin = true;
                            _serverCertificateValidator.DisableValidation();
                        }
                        else
                        {
                            throw new UserAbortException();
                        }
                    }
                } while (retryLogin);
            }
            return _session;
        }
    }
}