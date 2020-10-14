using Newtonsoft.Json;
using OpenBots.Core.Attributes.ClassAttributes;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Server.API_Methods;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenBots.Commands.Asset
{
    [Serializable]
    [Group("Asset Commands")]
    [Description("This command updates an Asset in OpenBots Server.")]
    public class UpdateAssetCommand : ScriptCommand
    {
        [PropertyDescription("Asset Name")]
        [InputSpecification("Enter the name of the Asset.")]
        [SampleUsage("Name || {vAssetName}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_AssetName { get; set; }

        [PropertyDescription("Asset Type")]
        [PropertyUISelectionOption("Text")]
        [PropertyUISelectionOption("Number")]
        [PropertyUISelectionOption("JSON")]
        [PropertyUISelectionOption("File")]
        [InputSpecification("Specify the type of the Asset.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_AssetType { get; set; }

        [PropertyDescription("Asset File Path")]
        [InputSpecification("Enter or Select the path of the file to upload.")]
        [SampleUsage(@"C:\temp\myfile.txt || {vFilePath} || {ProjectPath}\myfile.txt")]
        [Remarks("This input should only be used for File type Assets.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFileSelectionHelper)]
        public string v_AssetFilePath { get; set; }

        [PropertyDescription("Asset Value")]
        [InputSpecification("Enter the new value of the Asset.")]
        [SampleUsage("John || {vAssetValue}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_AssetValue { get; set; }

        [JsonIgnore]
        [NonSerialized]
        public List<Control> UploadPathControls;

        [JsonIgnore]
        [NonSerialized]
        public List<Control> AssetValueControls;

        public UpdateAssetCommand()
        {
            CommandName = "UpdateAssetCommand";
            SelectionName = "Update Asset";
            CommandEnabled = true;
            CustomRendering = true;
            v_AssetType = "Text";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vAssetName = v_AssetName.ConvertUserVariableToString(engine);
            var vAssetFilePath = v_AssetFilePath.ConvertUserVariableToString(engine);
            var vAssetValue = v_AssetValue.ConvertUserVariableToString(engine);

            var client = AuthMethods.GetAuthToken();
            var asset = AssetMethods.GetAsset(client, $"name eq '{vAssetName}' and type eq '{v_AssetType}'");

            if (asset == null)
                throw new Exception($"No Asset was found for '{vAssetName}' with type '{v_AssetType}'");

            switch (v_AssetType)
            {
                case "Text":
                    asset.TextValue = vAssetValue;
                    break;
                case "Number":
                    asset.NumberValue = double.Parse(vAssetValue);
                    break;
                case "JSON":
                    asset.JsonValue = vAssetValue;
                    break;
                case "File":
                    AssetMethods.UpdateFileAsset(client, asset, vAssetFilePath);
                    break;
            }

            if (v_AssetType != "File")
                AssetMethods.PutAsset(client, asset);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_AssetName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_AssetType", this, editor));
            ((ComboBox)RenderedControls[4]).SelectedIndexChanged += AssetTypeComboBox_SelectedValueChanged;

            UploadPathControls = new List<Control>();
            UploadPathControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_AssetFilePath", this, editor));
            foreach (var ctrl in UploadPathControls)
                ctrl.Visible = false;
            RenderedControls.AddRange(UploadPathControls);

            AssetValueControls = new List<Control>();
            AssetValueControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_AssetValue", this, editor));
            foreach (var ctrl in AssetValueControls)
                ctrl.Visible = false;
            RenderedControls.AddRange(AssetValueControls);

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            if (v_AssetType != "File")
                return base.GetDisplayValue() + $" ['{v_AssetName}' of Type '{v_AssetType}' With Value '{v_AssetValue}']";
            else
                return base.GetDisplayValue() + $" ['{v_AssetName}' of Type '{v_AssetType}' With File '{v_AssetFilePath}']";
        }

        private void AssetTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((ComboBox)RenderedControls[4]).Text == "File")
            {
                foreach (var ctrl in UploadPathControls)
                    ctrl.Visible = true;

                foreach (var ctrl in AssetValueControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }
            }
            else
            {
                foreach (var ctrl in UploadPathControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }

                foreach (var ctrl in AssetValueControls)
                    ctrl.Visible = true;
            }
        }
    }
}