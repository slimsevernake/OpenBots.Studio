using OpenBots.Core.Attributes.ClassAttributes;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using OpenBots.Server.Model;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Linq;

namespace OpenBots.Commands.Assets
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
        [PropertyUISelectionOption("File")]
        [InputSpecification("Specify the type of the Asset")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_AssetType { get; set; }

        [PropertyDescription("File Download Path")]
        [InputSpecification("Enter or Select the directory path to store the file in.")]
        [SampleUsage(@"C:\temp || {vDirectoryPath} || {ProjectPath}\temp")]
        [Remarks("This input should only be used for opening existing Documents.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFolderSelectionHelper)]
        public string v_FileDownloadPath { get; set; }

        [PropertyDescription("Output Asset Value Variable")]
        [InputSpecification("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        [JsonIgnore]
        [NonSerialized]
        public List<Control> DownloadPathControls;

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
            var vFileDownloadPath = v_FileDownloadPath.ConvertUserVariableToString(engine);

            string serverURL = "https://openbotsserver-dev.azurewebsites.net/";

            var client = new RestClient(serverURL);

            string token = GetAuthToken(client, "admin@admin.com", "Hello321");

            var assetList = GetAssets(client, token, $"name eq '{vAssetName}' and type eq '{v_AssetType}'");

            if (assetList.Count == 0)
                throw new Exception($"No Asset was found for '{vAssetName}' with type '{v_AssetType}'");

            var asset = assetList.FirstOrDefault();

            dynamic assetValue;
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
                    GetBinaryObject(client, token, binaryObjectID);
                    assetValue = "";
                    break;
                default:
                    assetValue = string.Empty;
                    break;
            }
            
            ((object)assetValue).StoreInUserVariable(engine, v_OutputUserVariableName);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_AssetName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_AssetType", this, editor));
            ((ComboBox)RenderedControls[4]).SelectedIndexChanged += AssetTypeComboBox_SelectedValueChanged;

            DownloadPathControls = new List<Control>();
            DownloadPathControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_FileDownloadPath", this, editor));

            foreach (var ctrl in DownloadPathControls)
                ctrl.Visible = false;

            RenderedControls.AddRange(DownloadPathControls);

            RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Get Asset From '{v_AssetName}' - Store Asset Value in '{v_OutputUserVariableName}'";
        }

        private void AssetTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((ComboBox)RenderedControls[4]).Text == "File")
            {
                foreach (var ctrl in DownloadPathControls)
                    ctrl.Visible = true;
            }
            else
            {
                foreach (var ctrl in DownloadPathControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }
            }
        }

        public string GetAuthToken(RestClient client, string email, string password)
        {           
            var request = new RestRequest("api/v1/auth/token", Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new {email, password});

            var response = client.Execute(request);
            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);
            return output["token"];
        }

        public List<Asset> GetAssets(RestClient client, string token, string filter)
        {
            client.RemoveDefaultParameter("Authorization");
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", token));
            var request = new RestRequest("api/v1/Assets", Method.GET);
            request.AddParameter("$filter", filter);
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);
            var items = output["items"];
            return JsonConvert.DeserializeObject<List<Asset>>(items);
        }

        public string GetBinaryObject(RestClient client, string token, Guid? binaryObjectID)
        {
            client.RemoveDefaultParameter("Authorization");
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", token));
            var request = new RestRequest("api/v1/BinaryObjects/{id}/download", Method.GET);
            request.AddUrlSegment("id", binaryObjectID.ToString());
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<BinaryObject>(response);
            //var items = output["items"];
            return "";
        }
    }
}