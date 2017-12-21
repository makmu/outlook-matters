using System;

namespace OutlookMatters.Core.Mattermost.v4.Interface
{
    public class MattermostException : Exception
    {
        public string Details { get; private set; }

        public MattermostException(Error error) : base(error.Message)
        {
            Details = error.DetailedError;
        }
    }
}
