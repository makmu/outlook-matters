using Microsoft.Office.Interop.Outlook;

namespace OutlookMatters.Mail
{
    public class OutlookMailExplorer: IMailExplorer
    {
        public MailData QuerySelectedMailData()
        {
            Explorer explorer = Globals.ThisAddIn.Application.ActiveExplorer();
            if (explorer == null)
            {
                throw new System.Exception("Could not find mail explorer object");
            }
            if (explorer.Selection == null)
            {
                throw new System.Exception("No selection information available");
            }
            if (explorer.Selection.Count == 0)
            {
                throw new System.Exception("No mails selected");
            }
            var item = explorer.Selection[1] as MailItem;
            if (item == null)
            {
                throw new System.Exception("Selected item is not a mail item");
            }
            return new MailData(item.SenderName, item.Subject, item.Body);
        }
    }
}