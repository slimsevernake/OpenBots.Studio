using Newtonsoft.Json;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Server.API_Methods;
using OpenBots.Core.Server.Models;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace OpenBots.Commands.Asset
{
	[Serializable]
	[Category("Asset Commands")]
	[Description("This command gets an Asset from OpenBots Server.")]
	public class GetAssetCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Asset Name")]
		[Description("Enter the name of the Asset.")]
		[SampleUsage("Name || {vAssetName}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_AssetName { get; set; }

		[Required]
		[DisplayName("Asset Type")]
		[PropertyUISelectionOption("Text")]
		[PropertyUISelectionOption("Number")]
		[PropertyUISelectionOption("JSON")]
		[PropertyUISelectionOption("File")]
		[Description("Specify the type of the Asset.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_AssetType { get; set; }

		[Required]
		[DisplayName("Output Directory Path")]
		[Description("Enter or Select the directory path to store the file in.")]
		[SampleUsage(@"C:\temp || {vDirectoryPath} || {ProjectPath}\temp")]
		[Remarks("This input should only be used for File type Assets.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		[Editor("ShowFolderSelectionHelper", typeof(UIAdditionalHelperType))]
		public string v_OutputDirectoryPath { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output Asset Value Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		[JsonIgnore]
		[Browsable(false)]
		private List<Control> _downloadPathControls;

		[JsonIgnore]
		[Browsable(false)]
		private List<Control> _outputVariableControls;

		public GetAssetCommand()
		{
			CommandName = "GetAssetCommand";
			SelectionName = "Get Asset";
			CommandEnabled = true;           
			v_AssetType = "Text";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			var vAssetName = v_AssetName.ConvertUserVariableToString(engine);
			var vOutputDirectoryPath = v_OutputDirectoryPath.ConvertUserVariableToString(engine);

			var client = AuthMethods.GetAuthToken();
			var asset = AssetMethods.GetAsset(client, $"name eq '{vAssetName}' and type eq '{v_AssetType}'");

			if (asset == null)
				throw new Exception($"No Asset was found for '{vAssetName}' with type '{v_AssetType}'");

			string assetValue = string.Empty;
			switch (v_AssetType)
			{
				case "Text":
					assetValue = asset.TextValue;
					break;
				case "Number":
					assetValue = asset.NumberValue.ToString();
					break;
				case "JSON":
					assetValue = asset.JsonValue;
					break;
				case "File":
					var binaryObjectID = asset.BinaryObjectID;
					BinaryObject binaryObject = BinaryObjectMethods.GetBinaryObject(client, binaryObjectID);      
					AssetMethods.DownloadFileAsset(client, asset.Id, vOutputDirectoryPath, binaryObject.Name);
					break;
				default:
					assetValue = string.Empty;
					break;
			}
			
			if (v_AssetType != "File")
				assetValue.StoreInUserVariable(engine, v_OutputUserVariableName);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_AssetName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_AssetType", this, editor));
			((ComboBox)RenderedControls[4]).SelectedIndexChanged += AssetTypeComboBox_SelectedValueChanged;

			_downloadPathControls = new List<Control>();
			_downloadPathControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_OutputDirectoryPath", this, editor));
			foreach (var ctrl in _downloadPathControls)
				ctrl.Visible = false;
			RenderedControls.AddRange(_downloadPathControls);

			_outputVariableControls = new List<Control>();
			_outputVariableControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_OutputUserVariableName", this, editor));
			foreach (var ctrl in _outputVariableControls)
				ctrl.Visible = false;
			RenderedControls.AddRange(_outputVariableControls);

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			if (v_AssetType != "File")
				return base.GetDisplayValue() + $" ['{v_AssetName}' of Type '{v_AssetType}'- Store Asset Value in '{v_OutputUserVariableName}']";
			else
				return base.GetDisplayValue() + $" ['{v_AssetName}' of Type '{v_AssetType}'- Save File in Directory '{v_OutputDirectoryPath}']";

		}

		private void AssetTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
		{
			if (((ComboBox)RenderedControls[4]).Text == "File")
			{
				foreach (var ctrl in _downloadPathControls)
					ctrl.Visible = true;

				foreach (var ctrl in _outputVariableControls)
				{
					ctrl.Visible = false;
					if (ctrl is TextBox)
						((TextBox)ctrl).Clear();
				}
			}
			else
			{
				foreach (var ctrl in _downloadPathControls)
				{
					ctrl.Visible = false;
					if (ctrl is TextBox)
						((TextBox)ctrl).Clear();
				}

				foreach (var ctrl in _outputVariableControls)
					ctrl.Visible = true;
			}
		}
	}
}