using System;
using OutlookMatters.Core.Chat;

namespace OutlookMatters.Core.Mattermost.v3
{
    public class ChatFactoryImpl : IChatFactory
    {
        public ISession NewInstance(Uri uri, string token, string userId)
        {
            throw new NotImplementedException();
        }
    }
}