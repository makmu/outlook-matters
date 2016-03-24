using OutlookMatters.Http;
using System;

namespace OutlookMatters.Mattermost.Session
{
    public class UserSessionFactory: ISessionFactory
    {
        private readonly IHttpClient _httpClient;

        public UserSessionFactory(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ISession CreateSession(Uri url, string token, string userId)
        {
            return new UserSession(url, token, userId, _httpClient);
        }
    }
}
