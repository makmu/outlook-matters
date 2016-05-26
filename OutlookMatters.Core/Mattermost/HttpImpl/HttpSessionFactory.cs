using System;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.Interface;

namespace OutlookMatters.Core.Mattermost.HttpImpl
{
    public class HttpSessionFactory : ISessionFactory
    {
        private readonly IHttpClient _httpClient;
        private readonly IRestService _restService;

        public HttpSessionFactory(IHttpClient httpClient, IRestService restService)
        {
            _httpClient = httpClient;
            _restService = restService;
        }

        public ISession CreateSession(Uri url, string token, string userId)
        {
            return new HttpSession(url, token, userId, _httpClient, _restService);
        }
    }
}