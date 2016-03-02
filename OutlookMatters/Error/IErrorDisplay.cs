using System;

namespace OutlookMatters.Error
{
    public interface IErrorDisplay
    {
        void Display(Exception exception);
    }
}