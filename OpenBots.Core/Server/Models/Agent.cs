using System;
using System.ComponentModel.DataAnnotations;

namespace OpenBots.Core.Server.Models
{
    public class Agent : NamedEntity
    {
        [MaxLength(100, ErrorMessage = "Name must be 100 characters or less.")]
        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters.")]
        [RegularExpression("^(?=.{3,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$")] // Alphanumeric with Underscore and Dot only
        [Display(Name = "Name")]
        public new string Name { get; set; }
        public Guid? MachineCredentials { get; set; }
        [Required]
        public string MachineName { get; set; }
        [Required]
        public bool IsEnabled { get; set; }
        public DateTime? LastReportedOn { get; set; }
        public string LastReportedStatus { get; set; }
        public string LastReportedWork { get; set; }
        public string LastReportedMessage { get; set; }
        public bool? IsHealthy { get; set; }
        [Required]
        public bool isConnected { get; set; }
        public Guid? CredentialId { get; set; }
    }
}
