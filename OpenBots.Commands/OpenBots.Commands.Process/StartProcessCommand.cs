using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Diagnostics = System.Diagnostics;

namespace OpenBots.Commands.Process
{
	[Serializable]
	[Category("Programs/Process Commands")]
	[Description("This command starts a program or process.")]

	public class StartProcessCommand : ScriptCommand
	{

		[Required]
		[DisplayName("Program Name or Path")]
		[Description("Provide a valid program name or enter a full path to the script/executable including the extension.")]
		[SampleUsage(@"notepad || excel || {vApp} || C:\temp\myapp.exe || {ProjectPath}\myapp.exe")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		[Editor("ShowFileSelectionHelper", typeof(UIAdditionalHelperType))]
		public string v_ProgramName { get; set; }

		[Required]
		[DisplayName("Arguments")]
		[Description("Enter any arguments or flags if applicable.")]
		[SampleUsage("-a || -version || {vArg}")]
		[Remarks("You will need to consult documentation to determine if your executable supports arguments or flags on startup.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_ProgramArgs { get; set; }

		[Required]
		[DisplayName("Wait For Exit")]
		[PropertyUISelectionOption("Yes")]
		[PropertyUISelectionOption("No")]
		[Description("Indicate whether to wait for the process to be completed.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_WaitForExit { get; set; }

		public StartProcessCommand()
		{
			CommandName = "StartProcessCommand";
			SelectionName = "Start Process";
			CommandEnabled = true;
			
			v_WaitForExit = "No";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			string vProgramName = v_ProgramName.ConvertUserVariableToString(engine);
			string vProgramArgs = v_ProgramArgs.ConvertUserVariableToString(engine);
			Diagnostics.Process newProcess;

			if (File.Exists(vProgramName))
				vProgramName = Path.GetFileNameWithoutExtension(vProgramName);

			if (string.IsNullOrEmpty(v_ProgramArgs))
				newProcess = Diagnostics.Process.Start(vProgramName);
			else
				newProcess = Diagnostics.Process.Start(vProgramName, vProgramArgs);			

			if (v_WaitForExit == "Yes")
				newProcess.WaitForExit();

			Thread.Sleep(2000);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_ProgramName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_ProgramArgs", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_WaitForExit", this, editor));

			return RenderedControls;
		}
		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Process '{v_ProgramName}' - Wait For Exit '{v_WaitForExit}']";
		}
	}
}