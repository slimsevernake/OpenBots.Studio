using OpenBots.Core.Attributes.PropertyAttributes;
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OpenBots.Commands.Image
{
	[Serializable]
	[Category("Image Commands")]
	[Description("This command takes a screenshot and saves it to a specified location.")]
	public class TakeScreenshotCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Window Name")]
		[Description("Select the name of the window to take a screenshot of.")]
		[SampleUsage("Untitled - Notepad || Current Window || {vWindow}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_WindowName { get; set; }

		[Required]
		[DisplayName("Image Location")]
		[Description("Enter or Select the path of the folder to save the image to.")]
		[SampleUsage(@"C:\temp || {vFolderPath} || {ProjectPath}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		[Editor("ShowFolderSelectionHelper", typeof(UIAdditionalHelperType))]
		public string v_FolderPath { get; set; }

		[Required]
		[DisplayName("Image File Name")]
		[Description("Enter or Select the name of the image file.")]
		[SampleUsage("myFile.png || {vFilename}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_FileName { get; set; }

		public TakeScreenshotCommand()
		{
			CommandName = "TakeScreenshotCommand";
			SelectionName = "Take Screenshot";
			CommandEnabled = true;

			v_WindowName = "Current Window";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			string windowName = v_WindowName.ConvertUserVariableToString(engine);
			string vFolderPath = v_FolderPath.ConvertUserVariableToString(engine);
			string vFileName = v_FileName.ConvertUserVariableToString(engine);
			string vFilePath = Path.Combine(vFolderPath, vFileName);

			Bitmap image;

			if (windowName == "Current Window")
				image = ImageMethods.Screenshot();
			else
				image = User32Functions.CaptureWindow(windowName);

			image.Save(vFilePath);
		}
		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultWindowControlGroupFor("v_WindowName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_FolderPath", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_FileName", this, editor));

			return RenderedControls;
		}


		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Target Window '{v_WindowName}' - Save to File Path '{v_FolderPath}\\{v_FileName}']";
		}
	}
}