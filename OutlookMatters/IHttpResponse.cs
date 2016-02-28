namespace OutlookMatters
{
    public interface IHttpResponse
    {
        string GetHeaderValue(string key);
        string GetPayload();
    }
}