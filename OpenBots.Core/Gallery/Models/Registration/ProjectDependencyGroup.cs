using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenBots.Core.Gallery.Models
{
    public class ProjectDependencyGroup
    {
        public string TargetFramework { get; set; }

        [JsonProperty("dependencies")]
        public List<ProjectDependency> ProjectDependencies { get; set; }
    }
}
