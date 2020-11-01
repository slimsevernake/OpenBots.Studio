using OpenBots.Core.Server.Interfaces;
using System;

namespace OpenBots.Core.Server.Models
{
    public abstract class Entity : IEntity
    {
        public Entity()
        {
            Id = Guid.NewGuid();
            Timestamp = new byte[1];
            CreatedBy = "";
            DeletedBy = "";
            IsDeleted = false;
        }

        public Guid? Id { get; set; }
        public bool? IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeleteOn { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
