﻿namespace OutlookMatters.Core.Http
{
    public interface IHttpRequest
    {
        IHttpRequest WithContentType(string contentType);
        IHttpRequest WithHeader(string key, string value);
        IHttpResponse Post(string payload);
        IHttpResponse Get();
    }
}