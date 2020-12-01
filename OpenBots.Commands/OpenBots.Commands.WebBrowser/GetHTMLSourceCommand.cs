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
using System.Net;
using System.Windows.Forms;

namespace OpenBots.Commands.WebBrowser
{
	[Serializable]
	[Category("Web Browser Commands")]
	[Description("This command downloads the HTML source of a web page for parsing without using browser automation.")]

	public class GetHTMLSourceCommand : ScriptCommand
	{
		[Required]
		[DisplayName("URL")]
		[Description("Enter a valid URL that you want to collect data from.")]
		[SampleUsage("http://mycompany.com/news || {vCompany}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_WebRequestURL { get; set; }

		[Required]
		[DisplayName("Execute Request As Logged On User")]
		[PropertyUISelectionOption("Yes")]
		[PropertyUISelectionOption("No")]
		[Description("Sets currently logged on user authentication information for the request.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_WebRequestCredentials { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output Response Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public GetHTMLSourceCommand()
		{
			CommandName = "GetHTMLSourceCommand";
			SelectionName = "Get HTML Source";
			CommandEnabled = true;            
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(v_WebRequestURL.ConvertUserVariableToString(engine));
			request.Method = "GET";
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
			if (v_WebRequestCredentials == "Yes")
			{
				request.Credentials = CredentialCache.DefaultCredentials;
			}

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string strResponse = reader.ReadToEnd();

			strResponse.StoreInUserVariable(engine, v_OutputUserVariableName);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_WebRequestURL", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_WebRequestCredentials", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Target URL '{v_WebRequestURL}' - Store Response in '{v_OutputUserVariableName}']";
		}
	}
}