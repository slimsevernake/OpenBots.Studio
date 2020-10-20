using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;

namespace OpenBots.Commands.Engine
{
    [Serializable]
    [Category("Engine Commands")]
    [Description("This command displays an engine context message to the user.")]
    public class ShowEngineContextCommand : ScriptCommand
    {
        [PropertyDescription("Close After X (Seconds)")]
        [InputSpecification("Specify how many seconds to display the message on screen. After the specified time," +
                            "\nthe message box will be automatically closed and script will resume execution.")]
        [SampleUsage("0 || 5 || {vSeconds})")]
        [Remarks("Set value to 0 to remain open indefinitely.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_AutoCloseAfter { get; set; }

        public ShowEngineContextCommand()
        {
            CommandName = "ShowEngineContextCommand";
            SelectionName = "Show Engine Context";
            CommandEnabled = true;
            CustomRendering = true;
            v_AutoCloseAfter = "0";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            int closeAfter = int.Parse(v_AutoCloseAfter.ConvertUserVariableToString(engine));

            if (engine.ScriptEngineUI == null)
            {
                MessageBox.Show(engine.GetEngineContext(), "Engine Context Command", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //automatically close messageboxes after 10 sec when closeAfter time is less than zero
            if (closeAfter < 0)
                v_AutoCloseAfter = "10";

            var result = ((Form)engine.ScriptEngineUI).Invoke(new Action(() =>
                {
                    engine.ScriptEngineUI.ShowEngineContext(engine.GetEngineContext(), closeAfter);
                }
            ));
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_AutoCloseAfter", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue();
        }
    }
}
