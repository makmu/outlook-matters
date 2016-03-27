using System.IO;
using System.Net;

namespace OutlookMatters.Http
{
    internal class HttpWebRequestAdaptor : IHttpRequest
    {
        private readonly HttpWebRequest _httpWebRequest;

        public HttpWebRequestAdaptor(HttpWebRequest httpWebRequest)
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
            PostPayload(payload);
            return GetResponse();
        }

        public IHttpResponse Get()
        {
            _httpWebRequest.Method = "GET";
            return GetResponse();
        }

        private void PostPayload(string payload)
        {
            _httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(_httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(payload);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private IHttpResponse GetResponse()
        {
            try
            {
                var httpResponse = (HttpWebResponse) _httpWebRequest.GetResponse();
                return new DefaultHttpResponse(httpResponse);
            }
            catch (WebException wex)
            {
                throw new HttpException(new DefaultHttpResponse((HttpWebResponse) wex.Response));
            }
        }
    }
}