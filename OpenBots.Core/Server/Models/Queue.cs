using OpenBots.Core.Server.Models;

namespace OpenBots.Core.Server.Models
{
    /// <summary>
    /// Queue Model
    /// </summary>
    public class Queue : NamedEntity
    {
        /// <summary>
        /// Describes the Queue
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Maximum number of QueueItem retries
        /// </summary>
        public int MaxRetryCount { get; set; }
    }
}
