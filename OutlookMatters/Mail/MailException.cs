namespace OutlookMatters.Mail
{
    public class MailException : System.Exception
    {
        public MailException(string message) : base(message)
        {
        }
    }
}