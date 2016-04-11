using Microsoft.Office.Interop.Outlook;

namespace OutlookMatters.Core.Mail
{
    public class OutlookExplorerService : IExplorerService
    {
        private readonly Application _application;

        public OutlookExplorerService(Application application)
        {
            _application = application;
        }

        public Explorer GetActiveExplorer()
        {
            return _application.ActiveExplorer();
        }
    }
}