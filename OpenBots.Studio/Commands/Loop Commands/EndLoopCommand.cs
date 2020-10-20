using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using OpenBots.Core.Command;
using OpenBots.Core.Infrastructure;

namespace OpenBots.Commands
{

    [Serializable]
    [Category("Loop Commands")]
    [Description("This command signifies the exit point of Loop command(s) and is required for all the Loop commands.")]
    public class EndLoopCommand : ScriptCommand
    {
        public EndLoopCommand()
        {
            DefaultPause = 0;
            CommandName = "EndLoopCommand";
            SelectionName = "End Loop";
            CommandEnabled = true;
            
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return "End Loop";
        }
    }
}