﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OutlookMatters.Core.Mattermost.v4.Interface
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChannelType
    {
        [EnumMember(Value = "O")] Public,
        [EnumMember(Value = "P")] Private,
        [EnumMember(Value = "D")] Direct
    }
}