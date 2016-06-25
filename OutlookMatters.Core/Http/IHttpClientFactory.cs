namespace OutlookMatters.Core.Http
{
    public interface IHttpClientFactory
    {
        IHttpClient CreateClient();
    }
}
