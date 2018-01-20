namespace OutlookMatters.Core.Http
{
    public class HttpClientFactory: IHttpClientFactory
    {
        public IHttpClient CreateClient()
        {
            return new DotNetHttpClient();
        }
    }
}
