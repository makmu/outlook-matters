using Microsoft.Office.Interop.Outlook;

namespace OutlookMatters.Core.Mail
{
    public interface IExplorerService
    {
        Explorer GetActiveExplorer();
    }
}