using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Session
{
    public interface IClientFactory
    {
        IClient GetClient(MattermostVersion version, LoginType loginType);
    }
}