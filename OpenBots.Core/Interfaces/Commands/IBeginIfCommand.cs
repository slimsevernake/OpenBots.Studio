using System.Data;

namespace OpenBots.Core.Infrastructure
{
    public interface IBeginIfCommand
    {
        string v_IfActionType { get; set; }
        DataTable v_IfActionParameterTable { get; set; }
    }
}
