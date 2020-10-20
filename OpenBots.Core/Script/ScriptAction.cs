using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using OpenBots.Core.Command;

namespace OpenBots.Core.Script
{
    public class ScriptAction
    {
        /// <summary>
        /// generic 'top-level' user-defined script command (ex. not nested)
        /// </summary>
        [Browsable(false)]
        public ScriptCommand ScriptCommand { get; set; }

        /// <summary>
        /// generic 'sub-level' commands (ex. nested commands within a loop)
        /// </summary>
        [Browsable(false)]
        public List<ScriptAction> AdditionalScriptCommands { get; set; }

        [JsonIgnore]
		[Browsable(false)]
        public bool IsExceptionIgnored { get; set; }
        /// <summary>
        /// adds a command as a nested command to a top-level command
        /// </summary>
        public ScriptAction AddAdditionalAction(ScriptCommand scriptCommand)
        {
            if (AdditionalScriptCommands == null)
                AdditionalScriptCommands = new List<ScriptAction>();

            ScriptAction newExecutionCommand = new ScriptAction() { ScriptCommand = scriptCommand };
            AdditionalScriptCommands.Add(newExecutionCommand);
            return newExecutionCommand;
        }
    }

}
