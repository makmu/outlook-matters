using Microsoft.Office.Interop.Outlook;

namespace OutlookMatters.Mail
{
    public interface IExplorerService
    {
        Explorer GetActiveExplorer();
    }
}