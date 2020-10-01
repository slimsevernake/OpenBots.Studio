using Newtonsoft.Json;
using OpenBots.Core.Attributes.ClassAttributes;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Server.API_Methods;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenBots.Commands.Asset
{
    [Serializable]
    [Group("Asset Commands")]
    [Description("This command gets an Asset from OpenBots Server")]
    public class GetAssetCommand : ScriptCommand
    {
        [PropertyDescription("Asset Name")]
        [InputSpecification("Enter the name of the Asset")]
        [SampleUsage("Name || {vAssetName}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_AssetName { get; set; }

        [PropertyDescription("Asset Type")]
        [PropertyUISelectionOption("Text")]
        [PropertyUISelectionOption("Number")]
        [PropertyUISelectionOption("JSON")]
        //[PropertyUISelectionOption("File")]
        [InputSpecification("Specify the type of the Asset")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_AssetType { get; set; }

        [PropertyDescription("Output Directory Path")]
        [InputSpecification("Enter or Select the directory path to store the file in.")]
        [SampleUsage(@"C:\temp || {vDirectoryPath} || {ProjectPath}\temp")]
        [Remarks("This input should only be used for File type Assets.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFolderSelectionHelper)]
        public string v_OutputDirectoryPath { get; set; }

        [PropertyDescription("Output Asset Value Variable")]
        [InputSpecification("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        [JsonIgnore]
        [NonSerialized]
        public List<Control> DownloadPathControls;

        [JsonIgnore]
        [NonSerialized]
        public List<Control> OutputVariableControls;

        public GetAssetCommand()
        {
            CommandName = "GetAssetCommand";
            SelectionName = "Get Asset";
            CommandEnabled = true;
            CustomRendering = true;
            v_AssetType = "Text";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vAssetName = v_AssetName.ConvertUserVariableToString(engine);
            var vOutputDirectoryPath = v_OutputDirectoryPath.ConvertUserVariableToString(engine);

            var client = new RestClient("https://openbotsserver-dev.azurewebsites.net/");

            AuthMethods.GetAuthToken(client, "admin@admin.com", "Hello321");

            var asset = AssetMethods.GetAsset(client, $"name eq '{vAssetName}' and type eq '{v_AssetType}'");

            if (asset == null)
                throw new Exception($"No Asset was found for '{vAssetName}' with type '{v_AssetType}'");

            string assetValue;
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
                    BinaryObjectMethods.GetBinaryObject(client, binaryObjectID);
                    assetValue = ""; //TODO Finish download for File Asset
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

            DownloadPathControls = new List<Control>();
            DownloadPathControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_OutputDirectoryPath", this, editor));
            foreach (var ctrl in DownloadPathControls)
                ctrl.Visible = false;
            RenderedControls.AddRange(DownloadPathControls);

            OutputVariableControls = new List<Control>();
            OutputVariableControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_OutputUserVariableName", this, editor));
            foreach (var ctrl in OutputVariableControls)
                ctrl.Visible = false;
            RenderedControls.AddRange(OutputVariableControls);

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            if (v_AssetType != "File")
                return base.GetDisplayValue() + $" [Get Asset '{v_AssetName}' of Type '{v_AssetType}'- Store Asset Value in '{v_OutputUserVariableName}']";
            else
                return base.GetDisplayValue() + $" [Get Asset '{v_AssetName}' of Type '{v_AssetType}'- Save File in Directory '{v_OutputDirectoryPath}']";

        }

        private void AssetTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((ComboBox)RenderedControls[4]).Text == "File")
            {
                foreach (var ctrl in DownloadPathControls)
                    ctrl.Visible = true;

                foreach (var ctrl in OutputVariableControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }
            }
            else
            {
                foreach (var ctrl in DownloadPathControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }

                foreach (var ctrl in OutputVariableControls)
                    ctrl.Visible = true;
            }
        }
    }
}