﻿using OpenBots.Core.Attributes.PropertyAttributes;
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
	[Description("This command closes an open window.")]
	public class CloseWindowCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Window Name")]
		[Description("Select the name of the window to close.")]
		[SampleUsage("Untitled - Notepad || Current Window || {vWindow}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_WindowName { get; set; }

		public CloseWindowCommand()
		{
			CommandName = "CloseWindowCommand";
			SelectionName = "Close Window";
			CommandEnabled = true;
			
			v_WindowName = "Current Window";          
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			string windowName = v_WindowName.ConvertUserVariableToString(engine);
			var targetWindows = User32Functions.FindTargetWindows(windowName);

			//loop each window
			foreach (var targetedWindow in targetWindows)
				User32Functions.CloseWindow(targetedWindow);
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