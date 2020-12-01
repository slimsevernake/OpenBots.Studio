using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.User32;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace OpenBots.Commands.Misc
{
    [Serializable]
    [Category("Misc Commands")]
    [Description("This command gets text from the user's clipboard.")]
    public class GetClipboardTextCommand : ScriptCommand
    {

        [Required]
        [Editable(false)]
        [DisplayName("Output Clipboard Text Variable")]
        [Description("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        public GetClipboardTextCommand()
        {
            CommandName = "GetClipboardTextCommand";
            SelectionName = "Get Clipboard Text";
            CommandEnabled = true;          
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            User32Functions.GetClipboardText().StoreInUserVariable(engine, v_OutputUserVariableName);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Store Clipboard Text in '{v_OutputUserVariableName}']";
        }
    }
}
