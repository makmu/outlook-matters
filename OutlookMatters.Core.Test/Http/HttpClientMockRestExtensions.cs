using System;
using Moq;
using OutlookMatters.Core.Http;

namespace Test.OutlookMatters.Core.Http
{
    public static class HttpClientMockRestExtensions
    {
        public static Mock<IHttpRequest> SetupRequest(this Mock<IHttpClient> httpClient, string baseUri, string path)
        {
            var httpRequest = new Mock<IHttpRequest>();
            httpClient.Setup(x => x.Request(new Uri(new Uri(baseUri), path)))
                .Returns(httpRequest.Object);
            return httpRequest;
        }

        public static Mock<IHttpRequest> WithToken(this Mock<IHttpRequest> httpRequest, string token)
        {
            httpRequest.Setup(x => x.WithHeader("Authorization", "Bearer " + token)).Returns(httpRequest.Object);
            return httpRequest;
        }

        public static Mock<IHttpResponse> Get(this Mock<IHttpRequest> httpRequest)
        {
            var httpResponse = new Mock<IHttpResponse>();
            httpRequest.Setup(x => x.Get()).Returns(httpResponse.Object);
            return httpResponse;
        }

        public static Mock<IHttpResponse> Post(this Mock<IHttpRequest> httpRequest, string payload)
        {
            var httpResponse = new Mock<IHttpResponse>();
            httpRequest.Setup(x => x.Post(payload)).Returns(httpResponse.Object);
            return httpResponse;
        }

        public static Mock<IHttpRequest> WithContentType(this Mock<IHttpRequest> httpRequest, string contentType)
        {
            httpRequest.Setup(x => x.WithContentType(contentType)).Returns(httpRequest.Object);
            return httpRequest;
        }

        public static Mock<IHttpResponse> Responses(this Mock<IHttpResponse> httpResponse, string payload)
        {
            httpResponse.Setup(x => x.GetPayload()).Returns(payload);
            return httpResponse;
        }

        public static Mock<IHttpResponse> WithToken(this Mock<IHttpResponse> httpResponse, string token)
        {
            httpResponse.Setup(x => x.GetHeaderValue("Token")).Returns(token);
            return httpResponse;
        }

        public static Mock<IHttpResponse> FailsAtGet(this Mock<IHttpRequest> httpRequest)
        {
            var httpResponse = new Mock<IHttpResponse>();
            var httpException = new HttpException(httpResponse.Object);
            httpRequest.Setup(x => x.Get()).Throws(httpException);
            return httpResponse;
        }

        public static Mock<IHttpResponse> FailsAtPost(this Mock<IHttpRequest> httpRequest, string payload)
        {
            var httpResponse = new Mock<IHttpResponse>();
            var httpException = new HttpException(httpResponse.Object);
            httpRequest.Setup(x => x.Post(payload)).Throws(httpException);
            return httpResponse;
        }
    }
}