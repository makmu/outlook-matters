namespace OutlookMatters.Http
{
    public interface IHttpResponse
    {
        string GetHeaderValue(string key);
        string GetPayload();
    }
}