using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenBots.Core.Gallery.Models
{
    public class Registration
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        public string Count { get; set; }

        [JsonProperty("items")]
        public List<RegistrationItem> Items { get; set; }

        public int TotalDownloads { get; set; }
    }
}
