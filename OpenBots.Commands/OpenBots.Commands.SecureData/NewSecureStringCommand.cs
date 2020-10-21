using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;

namespace OpenBots.Commands.SecureData
{
    [Serializable]
    [Category("Secure Data Commands")]
    [Description("This command adds text as a SecureString into a variable.")]
    public class NewSecureStringCommand : ScriptCommand
    {
        [Required]
		[DisplayName("Input Text")]
        [Description("Enter the text for the variable.")]
        [SampleUsage("Some Text || {vText}")]
        [Remarks("You can use variables in input if you encase them within braces {vText}. You can also perform basic math operations.")]
        [Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
        public string v_Input { get; set; }

        [Required]
        [Editable(false)]
        [DisplayName("Output SecureString Variable")]
        [Description("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        public NewSecureStringCommand()
        {
            CommandName = "NewSecureStringCommand";
            SelectionName = "New SecureString";
            CommandEnabled = true;          
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            SecureString secureStringValue = v_Input.ConvertUserVariableToString(engine).GetSecureString();

            secureStringValue.StoreInUserVariable(engine, v_OutputUserVariableName);           
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultPasswordInputGroupFor("v_Input", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Store SecureString in '{v_OutputUserVariableName}']";
        }
    }
}
