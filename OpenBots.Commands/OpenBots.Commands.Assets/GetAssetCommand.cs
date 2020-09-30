using OpenBots.Core.Attributes.ClassAttributes;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
        [InputSpecification("Specify whether to move or copy the selected emails.")]
        [SampleUsage("")]
        [Remarks("Moving will remove the emails from the original folder while copying will not.")]
        public string v_IMAPOperationType { get; set; }

        [PropertyDescription("Output Asset Value Variable")]
        [InputSpecification("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        public GetAssetCommand()
        {
            CommandName = "GetAssetCommand";
            SelectionName = "Get Asset";
            CommandEnabled = true;
            CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vAssetName = v_AssetName.ConvertUserVariableToString(engine);

            string serverURL = "https://openbotsserver-dev.azurewebsites.net/";

            var client = new RestClient(serverURL);

            string token = GetAuthToken(client, "admin@admin.com", "Hello321");

            string assetValue = GetAssetValue(client, token, $"name eq '{vAssetName}'");
            
            "".StoreInUserVariable(engine, v_OutputUserVariableName);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_AssetName", this, editor));

            RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Get Asset From '{v_AssetName}' - Store Asset Value in '{v_OutputUserVariableName}'";
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

        public string GetAssetValue(RestClient client, string token, string filter)
        {
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", token));
            var request = new RestRequest("api/v1/assets", Method.GET);
            request.AddParameter("$filter", filter);
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            return "";
        }
    }
}