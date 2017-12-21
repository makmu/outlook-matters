using System;
using OutlookMatters.Core.Chat;
using OutlookMatters.Core.Mattermost.v3.Interface;

namespace OutlookMatters.Core.Error
{
    public interface IErrorDisplay
    {
        void Display(MattermostException mex);
        void Display(Exception exception);
    }
}