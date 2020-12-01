using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Common;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.UI.Controls;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBots.Commands.Image
{
	[Serializable]
	[Category("Image Commands")]
	[Description("This command captures an image on screen and stores it as a Bitmap variable.")]
	public class CaptureImageCommand : ScriptCommand, IImageCommands
	{

		[Required]
		[DisplayName("Capture Search Image")]
		[Description("Use the tool to capture an image that will be located on screen during execution.")]
		[SampleUsage("")]
		[Remarks("Images with larger color variance will be found more quickly than those with a lot of white space. \n" +
				 "For images that are primarily white space, tagging color to the top-left corner of the image and setting \n" +
				 "the relative click position will produce faster results.")]
		[Editor("ShowImageCaptureHelper", typeof(UIAdditionalHelperType))]
		public string v_ImageCapture { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output Image Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("vUserVariable")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public CaptureImageCommand()
		{
			CommandName = "CaptureImageCommand";
			SelectionName = "Capture Image";
			CommandEnabled = true;
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;

			//user image to bitmap
			Bitmap capturedBmp = new Bitmap(Common.Base64ToImage(v_ImageCapture));
			capturedBmp.StoreInUserVariable(engine, v_OutputUserVariableName);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			UIPictureBox imageCapture = new UIPictureBox();
			imageCapture.Width = 200;
			imageCapture.Height = 200;
			imageCapture.DataBindings.Add("EncodedImage", this, "v_ImageCapture", false, DataSourceUpdateMode.OnPropertyChanged);

			RenderedControls.Add(commandControls.CreateDefaultLabelFor("v_ImageCapture", this));
			RenderedControls.AddRange(commandControls.CreateUIHelpersFor("v_ImageCapture", this, new Control[] { imageCapture }, editor));
			RenderedControls.Add(imageCapture);

			RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Capture Image on Screen - Store Image in '{v_OutputUserVariableName}']";
		}
	}
}
