using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Outlook;
using Newtonsoft.Json;
using OutlookMatters.Properties;
using Exception = System.Exception;
using Office = Microsoft.Office.Core;

namespace OutlookMatters
{
    [ComVisible(true)]
    public class MailItemContextMenuEntry : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI _ribbon;
        private User _user;
        private string _token;

        public string GetCustomUI(string ribbonId)
        {
            switch (ribbonId)
            {
                case "Microsoft.Outlook.Explorer":
                    return GetResourceText("OutlookMatters.MailItemContextMenuEntry.xml");
                default:
                    return string.Empty;
            }
        }

        public void OnSettingsClick(Office.IRibbonControl control)
        {
            var window = new View.SettingsWindow();
            window.ShowDialog();
        }

        public void OnPostClick(Office.IRibbonControl control)
        {
            HttpWebResponse httpResponse = GetLogInResponse();

            if (httpResponse != null)
            {
                PostMessage(httpResponse);
            }
        }
        

        private HttpWebResponse GetLogInResponse()
        {
            try
            {
                var url = new Uri(Settings.Default.MattermostUrl);
                url = new Uri(url, "api/v1/users/login");
                var login = new Login
                {
                    name = Settings.Default.TeamId,
                    email = Settings.Default.Email,
                    password = Settings.Default.Password
                };

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
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
                _token = httpWebResponse.Headers["Token"];
                return httpWebResponse;
            }
            catch (Exception e)
            {

                MessageBox.Show("Login to server failed: " + e, "Login Problem!");
            }

            return null;
        }

        private void PostMessage(HttpWebResponse httpResponse)
        {
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                _user = JsonConvert.DeserializeObject<User>(result);
            }
            
            var postUrl = PostUrl();
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(postUrl);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "Post";
            httpWebRequest.Headers["Authorization"] = "Bearer " + _token;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var post = GetPost();
                string json = JsonConvert.SerializeObject(post);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                MessageBox.Show("Done!", "Success");
            }
        }

        private Uri PostUrl()
        {
            string channelId = Settings.Default.ChannelId;
            string postUrl = "api/v1/channels/" + channelId + "/create";

            var url = new Uri(Settings.Default.MattermostUrl);
            url = new Uri(url, postUrl);
            return url;
        }

        private Post GetPost()
        {
            Explorer explorer = Globals.ThisAddIn.Application.ActiveExplorer();
            if (explorer != null && explorer.Selection != null && explorer.Selection.Count > 0)
            {
                object item = explorer.Selection[1];
                if (item is MailItem)
                {
                    MailItem mailItem = item as MailItem;
                    return new Post
                    {
                        channel_id = Settings.Default.ChannelId,
                        message = mailItem.Body,
                        user_id = _user.id
                    };
                }
            }
            return new Post
            {
                channel_id = Settings.Default.ChannelId,
                message = "Hello World from Outlook",
                user_id = _user.id
            };
        }

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (var resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}