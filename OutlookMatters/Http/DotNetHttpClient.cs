using System;
using System.Net;

namespace OutlookMatters.Http
{
    public class DotNetHttpClient : IHttpClient
    {
        public IHttpRequest Request(Uri url)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            return new HttpWebRequestAdaptor(httpWebRequest);
        }

        public IHttpRequest Get(Uri url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            return new HttpWebRequestAdaptor(httpWebRequest);
        }
    }
}