using System;

namespace OutlookMatters.Core.Http
{
    public interface IHttpClient
    {
        IHttpRequest Request(Uri url);
    }
}