using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace OpenBots.Commands.WebBrowser
{
	[Serializable]
	[Category("Web Browser Commands")]
	[Description("This command navigates forward in a Selenium web browser session.")]
	public class SeleniumNavigateForwardCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Browser Instance Name")]
		[Description("Enter the unique instance that was specified in the **Create Browser** command.")]
		[SampleUsage("MyBrowserInstance")]
		[Remarks("Failure to enter the correct instance name or failure to first call the **Create Browser** command will cause an error.")]
		public string v_InstanceName { get; set; }

		public SeleniumNavigateForwardCommand()
		{
			CommandName = "SeleniumNavigateForwardCommand";
			SelectionName = "Navigate Forward";
			CommandEnabled = true;
			
			v_InstanceName = "DefaultBrowser";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			var browserObject = v_InstanceName.GetAppInstance(engine);
			var seleniumInstance = (IWebDriver)browserObject;
			seleniumInstance.Navigate().Forward();
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InstanceName", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Instance Name '{v_InstanceName}']";
		}
	}
}