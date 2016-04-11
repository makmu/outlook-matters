using System;

namespace OutlookMatters.Core.Utils
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