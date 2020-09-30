using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenBots.Server.Model.Core
{
    public interface INamedEntity : IEntity, INameIDPair
    {
      
    }
}