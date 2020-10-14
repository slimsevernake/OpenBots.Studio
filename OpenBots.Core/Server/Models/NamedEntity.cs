using OpenBots.Core.Server.Interfaces;

namespace OpenBots.Core.Server.Models
{
    public abstract class NamedEntity : Entity, INamedEntity
    {
        public NamedEntity() : base()
        {

        }

        public string Name { get; set; }
    }
}
