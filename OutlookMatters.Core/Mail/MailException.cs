namespace OutlookMatters.Core.Mail
{
    public class MailException : System.Exception
    {
        public MailException(string message) : base(message)
        {
        }
    }
}