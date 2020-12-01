using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.User32;
using OpenBots.Core.Utilities.CommandUtilities;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Windows.Forms;

namespace OpenBots.Commands.Input
{
	[Serializable]
	[Category("Input Commands")]
	[Description("This command sends keystrokes to a targeted window.")]
	public class SendKeystrokesCommand : ScriptCommand, ISendKeystrokesCommand
	{

		[Required]
		[DisplayName("Window Name")]
		[Description("Select the name of the window to send keystrokes to.")]
		[SampleUsage("Untitled - Notepad || Current Window || {vWindow}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_WindowName { get; set; }

		[Required]
		[DisplayName("Text to Send")]
		[Description("Enter the text to be sent to the specified window.")]
		[SampleUsage("Hello, World! || {vText}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		[Editor("ShowEncryptionHelper", typeof(UIAdditionalHelperType))]
		public string v_TextToSend { get; set; }

		[Required]
		[DisplayName("Text Encrypted")]
		[PropertyUISelectionOption("Not Encrypted")]
		[PropertyUISelectionOption("Encrypted")]
		[Description("Indicate whether the text in *Text to Send* is encrypted.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_EncryptionOption { get; set; }

		public SendKeystrokesCommand()
		{
			CommandName = "SendKeystrokesCommand";
			SelectionName = "Send Keystrokes";
			CommandEnabled = true;
			

			v_WindowName = "Current Window";
			v_EncryptionOption = "Not Encrypted";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			var variableWindowName = v_WindowName.ConvertUserVariableToString(engine);

			if (variableWindowName != "Current Window")
				User32Functions.ActivateWindow(variableWindowName);

			string textToSend = v_TextToSend.ConvertUserVariableToString(engine);

			if (v_EncryptionOption == "Encrypted")
				textToSend = EncryptionServices.DecryptString(textToSend, "OPENBOTS");

			if (textToSend == "{WIN_KEY}")
			{
				User32Functions.KeyDown(Keys.LWin);
				User32Functions.KeyUp(Keys.LWin);
			}
			else if (textToSend.Contains("{WIN_KEY+"))
			{
				User32Functions.KeyDown(Keys.LWin);
				var remainingText = textToSend.Replace("{WIN_KEY+", "").Replace("}","");

				foreach (var c in remainingText)
				{
					Keys key = (Keys)Enum.Parse(typeof(Keys), c.ToString());
					User32Functions.KeyDown(key);
				}
				User32Functions.KeyUp(Keys.LWin);

				foreach (var c in remainingText)
				{
					Keys key = (Keys)Enum.Parse(typeof(Keys), c.ToString());
					User32Functions.KeyUp(key);
				}
			}
			else
				SendKeys.SendWait(textToSend);

			Thread.Sleep(500);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultWindowControlGroupFor("v_WindowName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_TextToSend", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_EncryptionOption", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Text '{v_TextToSend}' - Window '{v_WindowName}']";
		}     
	}
}