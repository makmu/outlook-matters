using System;

namespace OutlookMatters.Utils
{
    public class UserAbortException : Exception
    {
        public UserAbortException()
        {
        }

        public UserAbortException(string message) : base(message)
        {
        }
    }
}