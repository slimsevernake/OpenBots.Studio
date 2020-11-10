using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenBots.Core.Gallery.Models
{
    public class RegistrationItem
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        public string Count { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }
        [JsonProperty("items")]
        public List<RegistrationItemVersion> Items { get; set; }
    }
}
