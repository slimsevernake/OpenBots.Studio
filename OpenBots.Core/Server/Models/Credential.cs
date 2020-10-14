using System;

namespace OpenBots.Core.Server.Models
{
    public class Credential : NamedEntity
    {
        public string Provider { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string PasswordSecret { get; set; }
        public string PasswordHash { get; set; }
        public string Certificate { get; set; }
    }
}

