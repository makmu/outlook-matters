using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace OutlookMatters
{
    public class RestSession: ISession
    {
        private readonly Uri _baseUri;
        private readonly string _token;
        private readonly string _userId;

        public RestSession(Uri baseUri, string token, string userId)
        {
            _baseUri = baseUri;
            _token = token;
            _userId = userId;
        }

        public void CreatePost(string channelId, string message)
        {
            var postUrl = PostUrl(channelId);
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(postUrl);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "Post";
            httpWebRequest.Headers["Authorization"] = "Bearer " + _token;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var post = new Post {channel_id = channelId, message = message, user_id = _userId};
                string json = JsonConvert.SerializeObject(post);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
            }
        }

        private Uri PostUrl(string channelId)
        {
            string postUrl = "api/v1/channels/" + channelId + "/create";

            var url = new Uri(_baseUri, postUrl);
            return url;
        }
    }
}
