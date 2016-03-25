using System.IO;
using System.Net;

namespace OutlookMatters.Http
{
    internal class DefaultHttpResponse : IHttpResponse
    {
        private readonly HttpWebResponse _response;

        public DefaultHttpResponse(HttpWebResponse response)
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

        public void Dispose()
        {
            _response.Dispose();
        }
    }
}