using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenBots.Core.Gallery.Models
{
    public class CatalogEntry
    {
        public string Id { get; set; }
        public int Downloads { get; set; }
        public bool HasReadme { get; set; }
        public string ReleaseNotes { get; set; }
        public string RepositoryUrl { get; set; }
        public bool IsDeprecated { get; set; }
        public bool IsOfficial { get; set; }
        public bool IsVerified { get; set; }
        public bool IsFeatured { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Version { get; set; }
        public string Authors { get; set; }

        [JsonProperty("dependencyGroups")]
        public List<ProjectDependencyGroup> DependencyGroups { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string LicenseUrl { get; set; }
        public string ProjectUrl { get; set; }
        public string PackageContent { get; set; }
        public string Published { get; set; }
        public string Title { get; set; }

    }
}
