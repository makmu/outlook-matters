using System.Threading.Tasks;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Session
{
    public interface ISessionRepository
    {
        Task<ISession> RestoreSession();
    }
}