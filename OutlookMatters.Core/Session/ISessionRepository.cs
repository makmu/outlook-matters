using System.Threading.Tasks;
using OutlookMatters.Core.Chat;

namespace OutlookMatters.Core.Session
{
    public interface ISessionRepository
    {
        Task<ISession> RestoreSession();
    }
}