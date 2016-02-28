using System;

namespace OutlookMatters.Http
{
    public interface IHttpClient
    {
        IHttpRequest Post(Uri url);
    }
}
