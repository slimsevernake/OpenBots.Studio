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
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.Commands.Data
{
	[Serializable]
	[Category("Data Commands")]
	[Description("This command splits a string by a delimiter and saves the result in a list.")]
	public class SplitTextCommand : ScriptCommand
	{

		[Required]
		[DisplayName("Text Data")]
		[Description("Provide a variable or text value.")]
		[SampleUsage("Sample text, to be splitted by comma delimiter || {vTextData}")]
		[Remarks("Providing data of a type other than a 'String' will result in an error.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_InputText { get; set; }

		[Required]
		[DisplayName("Text Delimiter")]
		[Description("Specify the character that will be used to split the text.")]
		[SampleUsage("[crLF] || [chars] || , || {vDelimiter} || {vDelimeterList}")]
		[Remarks("[crLF] can be used for line breaks and [chars] can be used to split each character." +
				 "To use multiple delimeters, create a List variable of delimeter characters to use as the input.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_SplitCharacter { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output List Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public SplitTextCommand()
		{
			CommandName = "SplitTextCommand";
			SelectionName = "Split Text";
			CommandEnabled = true;         
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			var stringVariable = v_InputText.ConvertUserVariableToString(engine);

			string splitCharacter = "";
			List<string> splitCharacterList = new List<string>();
			bool isDelimeterList = false;

			dynamic input = v_SplitCharacter.ConvertUserVariableToString(engine);

			if (input == v_SplitCharacter && input.StartsWith("{") && input.EndsWith("}"))
				input = v_SplitCharacter.ConvertUserVariableToObject(engine);

			if (input is List<string>)
			{
				splitCharacterList = (List<string>)input;
				isDelimeterList = true;
			}
				
			else if (input is string)
				splitCharacter = (string)input;
			else
				throw new InvalidDataException($"{v_SplitCharacter} is not a valid delimeter");


			List<string> splitString;
			if (isDelimeterList)
			{
				splitString = stringVariable.Split(splitCharacterList.ToArray(), StringSplitOptions.None).ToList();
			}
			else
			{
				
				if (splitCharacter == "[crLF]")
				{
					splitString = stringVariable.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
				}
				else if (splitCharacter == "[chars]")
				{
					splitString = new List<string>();
					var chars = stringVariable.ToCharArray();
					foreach (var c in chars)
					{
						splitString.Add(c.ToString());
					}
				}
				else
				{
					splitString = stringVariable.Split(new string[] { splitCharacter }, StringSplitOptions.None).ToList();
				}
			}
			

			splitString.StoreInUserVariable(engine, v_OutputUserVariableName);           
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InputText", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_SplitCharacter", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Split '{v_InputText}' by '{v_SplitCharacter}' - Store List in '{v_OutputUserVariableName}']";
		}
	}
}