namespace OutlookMatters.Http
{
    public interface IHttpRequest
    {
        IHttpRequest WithContentType(string contentType);
        IHttpRequest WithHeader(string key, string value);
        IHttpResponse SendRequest(string payload);
        void Send(string payload);
    }
}