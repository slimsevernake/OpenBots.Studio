using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.User32;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;

namespace OpenBots.Commands.Window
{
    [Serializable]
    [Category("Window Commands")]
    [Description("This command sets a open window's state.")]
    public class SetWindowStateCommand : ScriptCommand
    {
        [Required]
		[DisplayName("Window Name")]
        [Description("Select the name of the window to set.")]
        [SampleUsage("Untitled - Notepad || Current Window || {vWindow}")]
        [Remarks("")]
        [Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
        public string v_WindowName { get; set; }

        [Required]
		[DisplayName("Window State")]
        [PropertyUISelectionOption("Maximize")]
        [PropertyUISelectionOption("Minimize")]
        [PropertyUISelectionOption("Restore")]
        [Description("Select the appropriate window state.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_WindowState { get; set; }

        public SetWindowStateCommand()
        {
            CommandName = "SetWindowStateCommand";
            SelectionName = "Set Window State";
            CommandEnabled = true;
            
            v_WindowName = "Current Window";
            v_WindowState = "Maximize";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            //convert window name
            string windowName = v_WindowName.ConvertUserVariableToString(engine);

            var targetWindows = User32Functions.FindTargetWindows(windowName);

            //loop each window and set the window state
            foreach (var targetedWindow in targetWindows)
            {
                WindowState WINDOW_STATE = WindowState.SwShowNormal;
                switch (v_WindowState)
                {
                    case "Maximize":
                        WINDOW_STATE = WindowState.SwMaximize;
                        break;

                    case "Minimize":
                        WINDOW_STATE = WindowState.SwMinimize;
                        break;

                    case "Restore":
                        WINDOW_STATE = WindowState.SwRestore;
                        break;

                    default:
                        break;
                }

                User32Functions.SetWindowState(targetedWindow, WINDOW_STATE);
            }     
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultWindowControlGroupFor("v_WindowName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_WindowState", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Window '{v_WindowName}' - Window State '{v_WindowState}']";
        }
    }
}