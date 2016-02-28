using System;
using System.Net;

namespace OutlookMatters
{
    public class DotNetHttpClient: IHttpClient
    {
        public IHttpRequest Post(Uri url)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            return new DotNetHttpRequest(httpWebRequest);
        }
    }
}
