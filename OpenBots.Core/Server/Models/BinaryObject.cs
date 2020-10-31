using System;

namespace OpenBots.Core.Server.Models
{
    public class BinaryObject : NamedEntity
    {
        public virtual Guid? OrganizationId { get; set; }
        public virtual string ContentType { get; set; }
        public virtual Guid? CorrelationEntityId { get; set; }
        public virtual string CorrelationEntity { get; set; }
        public virtual string Folder { get; set; }
        public virtual string StoragePath { get; set; }
        public virtual string StorageProvider { get; set; }
        public virtual long? SizeInBytes { get; set; }
        public virtual string HashCode { get; set; }
    }
}
