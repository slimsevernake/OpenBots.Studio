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
using System.Windows.Forms;

namespace OpenBots.Commands.Folder
{
	[Serializable]
	[Category("Folder Operation Commands")]
	[Description("This command renames an existing folder.")]
	public class RenameFolderCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Folder Path")]
		[Description("Enter or Select the path to the folder.")]
		[SampleUsage(@"C:\temp\myFolder || {ProjectPath}\myfolder || {vFolderPath}")]
		[Remarks("{ProjectPath} is the directory path of the current project.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		[Editor("ShowFolderSelectionHelper", typeof(UIAdditionalHelperType))] 
		public string v_SourceFolderPath { get; set; }

		[Required]
		[DisplayName("New Folder Name")]
		[Description("Specify the new folder name.")]
		[SampleUsage("New Folder Name || {vNewFolderName}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_NewName { get; set; }

		public RenameFolderCommand()
		{
			CommandName = "RenameFolderCommand";
			SelectionName = "Rename Folder";
			CommandEnabled = true;         
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			//apply variable logic
			var sourceFolder = v_SourceFolderPath.ConvertUserVariableToString(engine);
			var newFolderName = v_NewName.ConvertUserVariableToString(engine);

			//get source folder name and info
			DirectoryInfo sourceFolderInfo = new DirectoryInfo(sourceFolder);

			//create destination
			var destinationPath = Path.Combine(sourceFolderInfo.Parent.FullName, newFolderName);

			//rename folder
			Directory.Move(sourceFolder, destinationPath);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_SourceFolderPath", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_NewName", this, editor));
			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Rename '{v_SourceFolderPath}' to '{v_NewName}']";
		}
	}
}