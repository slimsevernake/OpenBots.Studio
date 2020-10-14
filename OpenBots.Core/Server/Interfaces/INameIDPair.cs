using System;

namespace OpenBots.Core.Server.Interfaces
{
    public interface INameIDPair
    {
        Guid? Id { get; set; }
        string Name { get; set; }
    }
}
