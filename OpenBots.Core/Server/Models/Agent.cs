using System;

namespace OpenBots.Core.Server.Models
{
    public class Agent : NamedEntity
    {
        public new string Name { get; set; }
        public Guid? MachineCredentials { get; set; }
        public string MachineName { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? LastReportedOn { get; set; }
        public string LastReportedStatus { get; set; }
        public string LastReportedWork { get; set; }
        public string LastReportedMessage { get; set; }
        public bool? IsHealthy { get; set; }
        public bool isConnected { get; set; }
        public Guid? CredentialId { get; set; }
    }
}
