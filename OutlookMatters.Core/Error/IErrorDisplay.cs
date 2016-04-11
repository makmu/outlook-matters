using System;
using OutlookMatters.Core.Mattermost;

namespace OutlookMatters.Core.Error
{
    public interface IErrorDisplay
    {
        void Display(MattermostException mex);
        void Display(Exception exception);
    }
}