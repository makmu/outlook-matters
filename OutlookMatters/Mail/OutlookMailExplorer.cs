using Microsoft.Office.Interop.Outlook;

namespace OutlookMatters.Mail
{
    public class OutlookMailExplorer: IMailExplorer
    {
        public string GetSelectedMailBody()
        {
            Explorer explorer = Globals.ThisAddIn.Application.ActiveExplorer();
            if (explorer != null && explorer.Selection != null && explorer.Selection.Count > 0)
            {
                object item = explorer.Selection[1];
                if (item is MailItem)
                {
                    MailItem mailItem = item as MailItem;
                    return mailItem.Body;
                }
            }
            return string.Empty;
        }
    }
}
