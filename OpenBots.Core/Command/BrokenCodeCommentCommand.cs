using OpenBots.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenBots.Core.Command
{
    [Serializable]
    [Category("Core Commands")]
    [Description("This command replaces broken code with an error comment.")]
    public class BrokenCodeCommentCommand : ScriptCommand
    {
        public BrokenCodeCommentCommand()
        {
            CommandName = "BrokenCodeCommentCommand";
            SelectionName = "Broken Code Comment";
            CommandEnabled = false;
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return $"Broken ['{v_Comment}']";
        }
    }
}