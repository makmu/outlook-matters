using System;

namespace OutlookMatters.Core.Chat
{
    public class ChatException : Exception
    {
        public ChatException(string errorMessage, Exception innerException = null) : base(errorMessage, innerException)
        {
        }
    }
}