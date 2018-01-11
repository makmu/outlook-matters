using System;
using OutlookMatters.Core.Chat;

namespace OutlookMatters.Core.Mattermost
{
    public interface IAuthenticationService
    {
        void Login(Uri baseUri, string username, string password, out string token);
    }
}