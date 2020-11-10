using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OpenBots.Core.Gallery.Models
{
    public class SearchResult
    {
        public string Index { get; set; }
        public DateTime LastReopen { get; set; }
        [JsonProperty("data")]
        public List<SearchResultPackage> Data { get; set; }
    }
}
