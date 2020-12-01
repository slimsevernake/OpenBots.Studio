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
using System.Windows.Forms;

namespace OpenBots.Commands.Misc
{
	[Serializable]
	[Category("Misc Commands")]
	[Description("This command displays a message to the user.")]
	public class ShowMessageCommand : ScriptCommand
	{

		[Required]
		[DisplayName("Message")]      
		[Description("Specify any text or variable value that should be displayed on screen.")]
		[SampleUsage("Hello World || {vMyText} || Hello {vName}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Message { get; set; }

		[Required]
		[DisplayName("Close After X (Seconds)")]
		[Description("Specify how many seconds to display the message on screen. After the specified time," + 
							"\nthe message box will be automatically closed and script will resume execution.")]
		[SampleUsage("0 || 5 || {vSeconds})")]
		[Remarks("Set value to 0 to remain open indefinitely.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_AutoCloseAfter { get; set; }

		public ShowMessageCommand()
		{
			CommandName = "MessageBoxCommand";
			SelectionName = "Show Message";
			CommandEnabled = true;          
			
			v_AutoCloseAfter = "0";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;

			int closeAfter = int.Parse(v_AutoCloseAfter.ConvertUserVariableToString(engine));

			dynamic variableMessage = v_Message.ConvertUserVariableToString(engine);

			if (variableMessage == v_Message && variableMessage.StartsWith("{") && variableMessage.EndsWith("}"))
				variableMessage = v_Message.ConvertUserVariableToObject(engine);

			string type = "";
			if (variableMessage != null)
				type = variableMessage.GetType().FullName;

			if (variableMessage is string)
				variableMessage = variableMessage.Replace("\\n", Environment.NewLine);
			else
				variableMessage = type + Environment.NewLine + StringMethods.ConvertObjectToString(variableMessage);

			if (engine.ScriptEngineUI == null)
			{
				engine.ReportProgress("Complex Messagebox Supported With UI Only");
				MessageBox.Show(variableMessage, "Message Box Command", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			//automatically close messageboxes for server requests
			if (engine.IsServerExecution && closeAfter <= 0)
				closeAfter = 10;

			var result = ((Form)engine.ScriptEngineUI).Invoke(new Action(() =>
				{
					engine.ScriptEngineUI.ShowMessage(variableMessage, "MessageBox", DialogType.OkOnly, closeAfter);
				}
			));

		}
		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Message", this, editor, 100, 300));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_AutoCloseAfter", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" ['{v_Message}']";
		}
	}
}
