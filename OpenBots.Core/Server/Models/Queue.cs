using OpenBots.Core.Server.Models;

namespace OpenBots.Core.Server.Models
{
    public class Queue : NamedEntity
    {
        public string Description { get; set; }
        public int MaxRetryCount { get; set; }
    }
}
