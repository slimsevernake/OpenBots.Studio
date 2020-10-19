using Emgu.CV;
using OpenBots.Core.Attributes.ClassAttributes;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace OpenBots.Commands
{
    [Serializable]
    [Group("Image Commands")]
    [Description("This command converts a video into images.")]
    public class ConvertVideoToImagesCommand : ScriptCommand
    {
        [PropertyDescription("Video File Path")]
        [InputSpecification("Enter or Select the path to the video.")]
        [SampleUsage(@"C:\temp\myfile.mp4 || {vFilePath} || {ProjectPath}\myfile.mp4")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFileSelectionHelper)]
        public string v_VideoFilePath { get; set; }

        [PropertyDescription("Images Output Path")]
        [InputSpecification("Enter or Select the path of the folder to save the Images to.")]
        [SampleUsage(@"C:\temp || {vFolderPath} || {ProjectPath}\temp")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFolderSelectionHelper)]
        public string v_FolderPath { get; set; }

        public ConvertVideoToImagesCommand()
        {
            CommandName = "ConvertVideoToImagesCommand";
            SelectionName = "Convert Video To Images";
            CommandEnabled = true;
            CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vVideoFilePath = v_VideoFilePath.ConvertUserVariableToString(engine);
            var vFolderPath = v_FolderPath.ConvertUserVariableToString(engine);

            if (!Directory.Exists(vFolderPath))
                Directory.CreateDirectory(vFolderPath);

            Mat prevImg = new Mat();
            using (var video = new VideoCapture(vVideoFilePath))
            using (var img = new Mat())
            {
                int count = 0;
                
                while (video.Grab())
                {
                    video.Retrieve(img);
                    if (prevImg.Equals(img))
                        break;

                    if  (count % 10 == 1)
                    {                       
                        var filename = Path.Combine(vFolderPath, string.Format("Frame_{0:yyyyMMdd_hhmmss.fff}.bmp", DateTime.Now));
                        CvInvoke.Imwrite(filename, img);
                    }
                    count += 1;
                    prevImg = img.Clone();
                }
            }
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_VideoFilePath", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_FolderPath", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Store Images in '{v_FolderPath}']";
        }
    }
}
