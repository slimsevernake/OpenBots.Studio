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
using System.IO;
using System.Windows.Forms;

namespace OpenBots.Commands.TextFile
{
	[Serializable]
	[Category("Text File Commands")]
	[Description("This command reads text data from a text file and stores it in a variable.")]
	public class ReadTextFileCommand : ScriptCommand
	{

		[Required]
		[DisplayName("Text File Path")]
		[Description("Enter or select the path to the text file.")]
		[SampleUsage(@"C:\temp\myfile.txt || {ProjectPath}\myText.txt || {vTextFilePath}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		[Editor("ShowFileSelectionHelper", typeof(UIAdditionalHelperType))]
		public string v_FilePath { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output Text Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public ReadTextFileCommand()
		{
			CommandName = "ReadTextFileCommand";
			SelectionName = "Read Text File";
			CommandEnabled = true;           
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			//convert variables
			var filePath = v_FilePath.ConvertUserVariableToString(engine);
			//read text from file
			var textFromFile = File.ReadAllText(filePath);
			//assign text to user variable
			textFromFile.StoreInUserVariable(engine, v_OutputUserVariableName);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_FilePath", this, editor));
			RenderedControls.AddRange(
				commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor)
			);

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Read Text From '{v_FilePath}' - Store Text in '{v_OutputUserVariableName}']";
		}
	}
}