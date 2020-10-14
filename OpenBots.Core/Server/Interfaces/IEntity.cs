using System;

namespace OpenBots.Core.Server.Interfaces
{
    public interface IEntity
    {
        string CreatedBy { get; set; }
        DateTime? CreatedOn { get; set; }
        string DeletedBy { get; set; }
        DateTime? DeleteOn { get; set; }
        Guid? Id { get; set; }
        bool? IsDeleted { get; set; }
        byte[] Timestamp { get; set; }
        DateTime? UpdatedOn { get; set; }
        string UpdatedBy { get; set; }
    }
}