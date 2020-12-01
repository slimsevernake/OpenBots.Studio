using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace OpenBots.Commands.ErrorHandling
{
	[Serializable]
	[Category("Error Handling Commands")]
	[Description("This command defines a catch block whose commands will execute if an exception is thrown from the " +
				 "associated try.")]
	public class CatchCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Exception Type")]
		[PropertyUISelectionOption("AccessViolationException")]
		[PropertyUISelectionOption("ArgumentException")]
		[PropertyUISelectionOption("ArgumentNullException")]
		[PropertyUISelectionOption("ArgumentOutOfRangeException")]
		[PropertyUISelectionOption("DivideByZeroException")]
		[PropertyUISelectionOption("Exception")]
		[PropertyUISelectionOption("FileNotFoundException")]
		[PropertyUISelectionOption("FormatException")]
		[PropertyUISelectionOption("IndexOutOfRangeException")]
		[PropertyUISelectionOption("InvalidDataException")]
		[PropertyUISelectionOption("InvalidOperationException")]
		[PropertyUISelectionOption("KeyNotFoundException")]
		[PropertyUISelectionOption("NotSupportedException")]
		[PropertyUISelectionOption("NullReferenceException")]
		[PropertyUISelectionOption("OverflowException")]
		[PropertyUISelectionOption("TimeoutException")]
		[Description("Select the appropriate exception type.")]
		[SampleUsage("")]
		[Remarks("This command will be executed if the type of the exception that occurred in the try block matches the selected exception type.")]
		public string v_ExceptionType { get; set; }

		public CatchCommand()
		{
			CommandName = "CatchCommand";
			SelectionName = "Catch";
			CommandEnabled = true;
			
			v_ExceptionType = "Exception";
		}

		public override void RunCommand(object sender)
		{
			//no execution required, used as a marker by the Automation Engine
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_ExceptionType", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" ({v_ExceptionType})";
		}
	}
}
