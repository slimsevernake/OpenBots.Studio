using System;

namespace OpenBots.Core.Server.Models
{
    public class QueueItem : NamedEntity
	{
		public bool IsLocked { get; set; }
		public DateTime? LockedOnUTC { get; set; }
		public DateTime? LockedUntilUTC { get; set; }
		public Guid? LockedBy { get; set; }
		public Guid? QueueId { get; set; }
		public string Type { get; set; }
		public string JsonType { get; set; }
		public string DataJson { get; set; }
		public string State { get; set; }
		public  string StateMessage { get; set; }
		public Guid? LockTransactionKey { get; set; }
		public DateTime? LockedEndTimeUTC { get; set; }
		public int RetryCount { get; set; }
		public int Priority { get; set; }
		public DateTime? ExpireOnUTC { get; set; }
		public DateTime? PostponeUntilUTC { get; set; }
		public string ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
		public string ErrorSerialized { get; set; }
	}
}