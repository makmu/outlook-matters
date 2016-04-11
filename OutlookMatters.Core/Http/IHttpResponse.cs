using System;

namespace OutlookMatters.Core.Http
{
    public interface IHttpResponse : IDisposable
    {
        string GetHeaderValue(string key);
        string GetPayload();
    }
}