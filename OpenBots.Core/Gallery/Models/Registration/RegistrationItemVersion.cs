using Newtonsoft.Json;

namespace OpenBots.Core.Gallery.Models
{
    public class RegistrationItemVersion
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        [JsonProperty("catalogEntry")]
        public CatalogEntry Catalog { get; set; }
    }
}
