namespace OutlookMatters.Mail
{
    public class MailData
    {
        public MailData(string senderName, string subject, string body)
        {
            SenderName = senderName;
            Subject = subject;
            Body = body;
        }

        public string SenderName { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
    }
}