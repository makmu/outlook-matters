using System;

namespace OutlookMatters.Core.Http
{
    public class ServiceException : Exception
    {
        public IHttpResponse Response { get; private set; }

        public ServiceException(IHttpResponse response)
        {
            Response = response;
        }
    }
}