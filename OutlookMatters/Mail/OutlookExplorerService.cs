using Microsoft.Office.Interop.Outlook;

namespace OutlookMatters.Mail
{
    class OutlookExplorerService : IExplorerService
    {
        public Explorer GetActiveExplorer()
        {
            return Globals.ThisAddIn.Application.ActiveExplorer();
        }
    }
}