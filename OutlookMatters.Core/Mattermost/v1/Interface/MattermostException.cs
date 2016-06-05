using System;

namespace OutlookMatters.Core.Mattermost.v1.Interface
{
    public class MattermostException : Exception
    {
        public string Details { get; private set; }

        public MattermostException(Error error) : base(error.message)
        {
            Details = error.detailed_error;
        }
    }
}