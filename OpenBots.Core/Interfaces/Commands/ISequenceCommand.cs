using OpenBots.Core.Command;
using System.Collections.Generic;

namespace OpenBots.Core.Infrastructure
{
    public interface ISequenceCommand
    {
        List<ScriptCommand> ScriptActions { get; set; }
        string v_Comment { get; set; }
        string GetDisplayValue();
    }
}
