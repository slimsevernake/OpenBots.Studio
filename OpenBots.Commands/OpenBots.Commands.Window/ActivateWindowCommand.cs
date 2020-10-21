using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.User32;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace OpenBots.Commands.Window
{
    [Serializable]
    [Category("Window Commands")]
    [Description("This command activates an open window and brings it to the front.")]
    public class ActivateWindowCommand : ScriptCommand
    {
        [Required]
		[DisplayName("Window Name")]
        [Description("Select the name of the window to activate and bring forward.")]
        [SampleUsage("Untitled - Notepad || {vWindow}")]
        [Remarks("")]
        [Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
        public string v_WindowName { get; set; }

        public ActivateWindowCommand()
        {
            CommandName = "ActivateWindowCommand";
            SelectionName = "Activate Window";
            CommandEnabled = true;           
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            string windowName = v_WindowName.ConvertUserVariableToString(engine);

            User32Functions.ActivateWindow(windowName);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultWindowControlGroupFor("v_WindowName", this, editor));

            return RenderedControls;
        }     

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Window '{v_WindowName}']";
        }
    }
}