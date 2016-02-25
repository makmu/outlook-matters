using System;

namespace OutlookMatters
{
    public class RestSessionFactory: ISessionFactory
    {
        public ISession CreateSession(Uri url, string token, string userId)
        {
            return new RestSession(url, token, userId);
        }
    }
}
