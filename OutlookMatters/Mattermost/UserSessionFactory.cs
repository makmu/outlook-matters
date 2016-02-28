using System;
using OutlookMatters.Http;

namespace OutlookMatters.Mattermost
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
