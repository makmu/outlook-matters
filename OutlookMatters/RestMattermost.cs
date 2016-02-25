using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace OutlookMatters
{
    public class RestMattermost: IMattermost
    {
        private readonly ISessionFactory _sessionFactory;

        public RestMattermost(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ISession LoginByUsername(string url, string teamId, string username, string password)
        {
            var loginUrl = new Uri(new Uri(url), "api/v1/users/login");
            var login = new Login
            {
                name = teamId,
                email = username,
                password = password
            };

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "Post";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(login);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var token = httpWebResponse.Headers["Token"];

            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                var user = JsonConvert.DeserializeObject<User>(result);
                return _sessionFactory.CreateSession(new Uri(url), token, user.id);
            }
        }
    }
}
