using System;
using OutlookMatters.Mattermost;

namespace OutlookMatters.Error
{
    public interface IErrorDisplay
    {
        void Display(MattermostException mex);
        void Display(Exception exception);
    }
}