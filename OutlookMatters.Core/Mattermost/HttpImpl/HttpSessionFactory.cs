using System;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpSessionFactory : ISessionFactory
    {
        private readonly IHttpClient _httpClient;

        public HttpSessionFactory(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ISession CreateSession(Uri url, string token, string userId)
        {
            return new HttpSession(url, token, userId, _httpClient);
        }
    }
}