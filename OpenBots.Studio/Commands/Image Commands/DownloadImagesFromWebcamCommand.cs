using OpenBots.Core.Attributes.ClassAttributes;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using OpenBots.UI.Forms.Supplement_Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace OpenBots.Commands
{
    [Serializable]
    [Group("Image Commands")]
    [Description("This command captures and stores images from a webcam stream.")]
    public class DownloadImagesFromWebcamCommand : ScriptCommand
    {
        [PropertyDescription("Images Output Path")]
        [InputSpecification("Enter or Select the path of the folder to save the Images to.")]
        [SampleUsage(@"C:\temp || {vFolderPath} || {ProjectPath}\temp")]
        [Remarks("Images will be saved at a rate of 1 frame per second while the webcam window is open.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFolderSelectionHelper)]
        public string v_FolderPath { get; set; }

        public DownloadImagesFromWebcamCommand()
        {
            CommandName = "DownloadImagesFromWebcamCommand";
            SelectionName = "Download Images From Webcam";
            CommandEnabled = true;
            CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vFolderPath = v_FolderPath.ConvertUserVariableToString(engine);

            if (!Directory.Exists(vFolderPath))
                Directory.CreateDirectory(vFolderPath);

            frmWebcam displayManager = new frmWebcam(vFolderPath);
            displayManager.ShowDialog();
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_FolderPath", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Store Images in '{v_FolderPath}']";
        }
    }
}
