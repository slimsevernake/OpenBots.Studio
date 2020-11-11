using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenBots.Core.Gallery.Models
{
    public class Feed
    {
        public string Version { get; set; }

        [JsonProperty("resources")]
        public List<Resource> Resources { get; set; }
    }
}
