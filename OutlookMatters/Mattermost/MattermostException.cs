﻿using System;

namespace OutlookMatters.Mattermost
{
    public class MattermostException : Exception
    {
        public string Details { get; private set; }

        public MattermostException(DataObjects.Error error) : base(error.message)
        {
            Details = error.detailed_error;
        }
    }
}