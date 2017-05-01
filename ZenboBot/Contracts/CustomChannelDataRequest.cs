using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zenbo.BotService.Contracts
{
    [Serializable]
    public class CognitiveServicesRequest
    {
        [JsonProperty(@"imageid")]
        public string ImageId { get; set; }

        [JsonProperty(@"services")]
        public List<string> ServicesToHit { get; set; }
    }

    [Serializable]
    public class CustomChannelDataRequest
    {
        [JsonProperty(@"cognitiveRequest")]
        public CognitiveServicesRequest cognitiveRequest { get; set; }
    }
}