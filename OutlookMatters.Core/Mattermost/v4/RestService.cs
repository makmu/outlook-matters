using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public class RestService : IRestService
    {
        private IHttpClient _httpClient;

        public RestService(IHttpClient _httpClient)
        {
            this._httpClient = _httpClient;
        }
    }
}
