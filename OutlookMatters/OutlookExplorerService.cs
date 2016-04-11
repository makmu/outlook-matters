using System;
using Microsoft.Office.Interop.Outlook;
using OutlookMatters.Core.Mail;

namespace OutlookMatters
{
    public class OutlookExplorerService : IExplorerService
    {
        public Explorer GetActiveExplorer()
        {
            return Globals.ThisAddIn.Application.ActiveExplorer();
        }
    }
}