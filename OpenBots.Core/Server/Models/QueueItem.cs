using OpenBots.Server.Model.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OpenBots.Server.Model
{
	/// <summary>
	/// QueueItem Model
	/// </summary>
	public class QueueItem : NamedEntity
	{
		/// <summary>
		/// Whether a QueueItem is locked by a job or not
		/// </summary>
		public bool IsLocked { get; set; }

		/// <summary>
		/// When the QueueItem was locked
		/// </summary>
		public DateTime? LockedOn { get; set; }

		/// <summary>
		/// When to lock QueueItem if still being executed
		/// </summary>
		public DateTime? LockedUntil { get; set; }

		/// <summary>
		/// Which Agent locked the QueueItem
		/// </summary>
		public Guid? LockedBy { get; set; }

		/// <summary>
		/// Which Queue the QueueItem belongs to
		/// </summary>
		public Guid QueueId { get; set; }

		/// <summary>
		/// Format of Data
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Describes the type of item the queue is dealing with
		/// </summary>
		public string JsonType { get; set; }

		/// <summary>
		/// Data in JSON or Text format
		/// </summary>
		public string DataJson { get; set; }

		/// <summary>
		/// Failed, Expired, Successful, New
		/// </summary>
		public string State { get; set; }

		/// <summary>
		/// Message given to user after state of QueueItem was changed
		/// </summary>
		public  string StateMessage { get; set; }

		/// <summary>
		/// Guid generated when item is dequeued
		/// </summary>
		public Guid? LockTransactionKey { get; set; }

		/// <summary>
		/// Tells when QueueItem has been executed and when IsLocked as been turned back to false
		/// </summary>
		public DateTime? LockedEndTime { get; set; }
		/// <summary>
		/// Number of time a QueueItem has been retried
		/// </summary>
		public int RetryCount { get; set; }
		/// <summary>
		/// Priority of when queue item should be dequeued
		/// </summary>
		public int Priority { get; set; }
	}
}