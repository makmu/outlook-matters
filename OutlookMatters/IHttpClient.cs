using System;

namespace OutlookMatters
{
    public interface IHttpClient
    {
        IHttpRequest Post(Uri url);
    }
}
