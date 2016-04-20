using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public class Payload
    {
        [JsonProperty("id")]
        public string PostId { get; set; } 
    }
}
