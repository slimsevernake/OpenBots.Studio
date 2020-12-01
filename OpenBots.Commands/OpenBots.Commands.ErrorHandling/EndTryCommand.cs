using OpenBots.Core.Command;
using OpenBots.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenBots.Commands.ErrorHandling
{
    [Serializable]
    [Category("Error Handling Commands")]
    [Description("This command specifies the end of a try/catch block.")]
    public class EndTryCommand : ScriptCommand
    {
        public EndTryCommand()
        {
            CommandName = "EndTryCommand";
            SelectionName = "End Try";
            CommandEnabled = true;          
        }

        public override void RunCommand(object sender)
        {
            //no execution required, used as a marker by the Automation Engine
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue();
        }
    }
}