using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace OutlookMatters.Core.Mattermost
{
    [JsonConverter(typeof (StringEnumConverter))]
    public enum ChannelType
    {
        [EnumMember(Value = "O")] Public,
        [EnumMember(Value = "P")] Private,
        [EnumMember(Value = "D")] Direct
    }
}