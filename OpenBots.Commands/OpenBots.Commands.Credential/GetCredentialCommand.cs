using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Server.API_Methods;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;
using System.ComponentModel;

namespace OpenBots.Commands.Credential
{
    [Serializable]
    [Category("Credential Commands")]
    [Description("This command gets a Credential from OpenBots Server.")]
    public class GetCredentialCommand : ScriptCommand
    {
        [DisplayName("Credential Name")]
        [InputSpecification("Enter the name of the Credential.")]
        [SampleUsage("Name || {vCredentialName}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_CredentialName { get; set; }

        [DisplayName("Output Username Variable")]
        [InputSpecification("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        [DisplayName("Output Password Variable")]
        [InputSpecification("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName2 { get; set; }

        public GetCredentialCommand()
        {
            CommandName = "GetCredentialCommand";
            SelectionName = "Get Credential";
            CommandEnabled = true;
            CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vCredentialName = v_CredentialName.ConvertUserVariableToString(engine);

            var client = AuthMethods.GetAuthToken();
            var credential = CredentialMethods.GetCredential(client, $"name eq '{vCredentialName}'");

            if (credential == null)
                throw new Exception($"No Credential was found for '{vCredentialName}'");

            string username = credential.UserName;
            SecureString password = credential.PasswordSecret.GetSecureString();

            username.StoreInUserVariable(engine, v_OutputUserVariableName);
            password.StoreInUserVariable(engine, v_OutputUserVariableName2);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_CredentialName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName2", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
              return base.GetDisplayValue() + $" ['{v_CredentialName}' - Store Username in '{v_OutputUserVariableName}' and Password in '{v_OutputUserVariableName2}']";
        }       
    }
}