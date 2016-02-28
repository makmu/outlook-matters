using System.IO;
using System.Net;

namespace OutlookMatters
{
    internal class DotNetHttpRequest : IHttpRequest
    {
        private readonly HttpWebRequest _httpWebRequest;

        public DotNetHttpRequest(HttpWebRequest httpWebRequest)
        {
            _httpWebRequest = httpWebRequest;
        }

        public IHttpRequest WithContentType(string contentType)
        {
            _httpWebRequest.ContentType = contentType;
            return this;
        }

        public IHttpRequest WithHeader(string key, string value)
        {
            _httpWebRequest.Headers[key] = value;
            return this;
        }

        public IHttpResponse Send(string payload)
        {
            using (var streamWriter = new StreamWriter(_httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(payload);
                streamWriter.Flush();
                streamWriter.Close();
            }
            return new DotNetHttpResponse(_httpWebRequest.GetResponse());
        }
    }

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
            using (var streamReader = new StreamReader(_response.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}