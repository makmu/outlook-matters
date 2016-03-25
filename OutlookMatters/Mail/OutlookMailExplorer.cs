using Microsoft.Office.Interop.Outlook;
using Exception = System.Exception;

namespace OutlookMatters.Mail
{
    public class OutlookMailExplorer : IMailExplorer
    {
        public MailData QuerySelectedMailData()
        {
            var explorer = Globals.ThisAddIn.Application.ActiveExplorer();
            if (explorer == null)
            {
                throw new Exception("Could not find mail explorer object");
            }
            if (explorer.Selection == null)
            {
                throw new Exception("No selection information available");
            }
            if (explorer.Selection.Count == 0)
            {
                throw new Exception("No mails selected");
            }
            var item = explorer.Selection[1] as MailItem;
            if (item == null)
            {
                throw new Exception("Selected item is not a mail item");
            }
            return new MailData(item.SenderName, item.Subject, item.Body);
        }
    }
}