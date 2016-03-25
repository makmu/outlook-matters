using System;

namespace OutlookMatters.Http
{
    public interface IHttpResponse: IDisposable
    {
        string GetHeaderValue(string key);
        string GetPayload();
    }
}