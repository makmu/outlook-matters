using System;

namespace OutlookMatters.Core.Mattermost.Interface
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