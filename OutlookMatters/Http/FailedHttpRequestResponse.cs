using System.IO;
using System.Net;

namespace OutlookMatters.Http
{
    internal class FailedHttpRequestResponse : IHttpResponse
    {
        private readonly HttpWebResponse _response;

        public FailedHttpRequestResponse(HttpWebResponse response)
        {
            _response = response;
        }

        public string GetHeaderValue(string key)
        {
            return _response.Headers[key];
        }

        public string GetPayload()
        {
            string errorPayload;
            using (_response)
            {
                using (var streamReader = new StreamReader(_response.GetResponseStream()))
                {
                    errorPayload = streamReader.ReadToEnd();
                }
            }
            throw new WebException(errorPayload);
        }

        public void Dispose()
        {
            _response.Dispose();
        }
    }
}