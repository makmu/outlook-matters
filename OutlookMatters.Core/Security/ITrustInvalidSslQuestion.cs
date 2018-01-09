namespace OutlookMatters.Core.Security
{
    public interface ITrustInvalidSslQuestion
    {
        bool GetAnswer(string url, string message);
    }
}
