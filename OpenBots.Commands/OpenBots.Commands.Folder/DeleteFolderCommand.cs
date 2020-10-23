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
	[Description("This command deletes a folder from a specified location.")]
	public class DeleteFolderCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Folder Path")]
		[Description("Enter or Select the path to the folder.")]
		[SampleUsage(@"C:\temp\myfolder || {ProjectPath}\myfolder  || {vTextFolderPath}")]
		[Remarks("{ProjectPath} is the directory path of the current project.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		[Editor("ShowFolderSelectionHelper", typeof(UIAdditionalHelperType))] 
		public string v_SourceFolderPath { get; set; }

		public DeleteFolderCommand()
		{
			CommandName = "DeleteFolderCommand";
			SelectionName = "Delete Folder";
			CommandEnabled = true;          
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			//apply variable logic
			var sourceFolder = v_SourceFolderPath.ConvertUserVariableToString(engine);

			//delete folder
			Directory.Delete(sourceFolder, true);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_SourceFolderPath", this, editor));
			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Delete '{v_SourceFolderPath}']";
		}
	}
}