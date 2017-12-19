using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OutlookMatters.Core.Http;
using OutlookMatters.Core.Mattermost.v4.Interface;

namespace OutlookMatters.Core.Mattermost.v4
{
    public class RestService : IRestService
    {
        private readonly IHttpClient _httpClient;

        public RestService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Login(Uri baseUri, Login login, out string token)
        {
            var loginUrl = new Uri(baseUri, "api/v4/users/login");
            using (var response = _httpClient.Request(loginUrl)
                .WithContentType("text/json")
                .Post(JsonConvert.SerializeObject(login)))
            {
                token = response.GetHeaderValue("Token");
            }
        }

        public IEnumerable<Team> GetTeams(Uri baseUri, string token)
        {
            throw new NotImplementedException();
        }
    }
}
