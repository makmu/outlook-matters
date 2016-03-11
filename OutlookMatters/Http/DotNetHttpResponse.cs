using System.IO;
using System.Net;

namespace OutlookMatters.Http
{
    internal class DotNetHttpResponse : IHttpResponse
    {
        private readonly WebResponse _response;

        public DotNetHttpResponse(WebResponse response)
        {
            _response = response;
        }

        public string GetHeaderValue(string key)
        {
            return _response.Headers[key];
        }

        public string GetPayload()
        {
            using (_response)
            {
                using (var streamReader = new StreamReader(_response.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}