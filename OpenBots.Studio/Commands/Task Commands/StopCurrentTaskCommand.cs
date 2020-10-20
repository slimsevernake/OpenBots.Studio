using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OpenBots.Core.Command;
using OpenBots.Core.Infrastructure;

namespace OpenBots.Commands
{
    [Serializable]
    [Category("Task Commands")]
    [Description("This command stops the currently running task.")]
    public class StopCurrentTaskCommand : ScriptCommand
    {
        public StopCurrentTaskCommand()
        {
            CommandName = "StopCurrentTaskCommand";
            SelectionName = "Stop Current Task";
            CommandEnabled = true;           
        }

        public override void RunCommand(object sender)
        {
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