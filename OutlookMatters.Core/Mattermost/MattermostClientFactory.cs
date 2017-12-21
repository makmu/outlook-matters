using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.v3;
using OutlookMatters.Core.Mattermost.v4;
using OutlookMatters.Core.Session;
using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Mattermost
{
    public class MattermostClientFactory : IClientFactory
    {
        private readonly IHttpClient _httpClient;

        public MattermostClientFactory(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IClient GetClient(MattermostVersion version)
        {
            switch (version)
            {
                case MattermostVersion.ApiVersionThree:
                    var restService3 = new RestServiceImpl(_httpClient);
                    var sessionFactory3 = new ChatFactoryImpl();
                    return new ClientImpl(restService3, sessionFactory3);

                case MattermostVersion.ApiVersionFour:
                    var restService4 = new RestService(_httpClient);
                    var sessionFactory4 = new ChatFactory();
                    return new Client(restService4, sessionFactory4);

                default:
                    throw new ArgumentOutOfRangeException("version");
            }
        }
    }
}