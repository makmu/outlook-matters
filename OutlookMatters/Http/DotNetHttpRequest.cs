using System.IO;
using System.Net;

namespace OutlookMatters.Http
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

        public IHttpResponse Post(string payload)
        {
            var response = PostPayload(payload);
            return new DotNetHttpResponse(response);
        }

        public void PostAndForget(string payload)
        {
            var response = PostPayload(payload);
            DiscardResponseAndFreeConnection(response);
        }

        public IHttpResponse Get()
        {
            _httpWebRequest.Method = "GET";
            return new DotNetHttpResponse(_httpWebRequest.GetResponse());
        }

        private static void DiscardResponseAndFreeConnection(WebResponse response)
        {
            using (response)
            {
                // do nothing
            }
        }

        private WebResponse PostPayload(string payload)
        {
            _httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(_httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(payload);
                streamWriter.Flush();
                streamWriter.Close();
            }
            return _httpWebRequest.GetResponse();
        }
    }
}