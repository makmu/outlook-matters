using System;

namespace OutlookMatters.Core.Http
{
    public class HttpException : Exception
    {
        public IHttpResponse Response { get; private set; }

        public HttpException(IHttpResponse response)
        {
            Response = response;
        }
    }
}