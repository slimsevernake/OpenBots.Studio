﻿using System.ComponentModel;
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

namespace OpenBots.Commands.Credential
{
    [Serializable]
    [Category("Credential Commands")]
    [Description("This command updates a Credential in OpenBots Server.")]
    public class UpdateCredentialCommand : ScriptCommand
    {
        [DisplayName("Credential Name")]
        [Description("Enter the name of the Credential.")]
        [SampleUsage("Name || {vCredentialName}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_CredentialName { get; set; }

        [DisplayName("Credential Username")]
        [Description("Enter the Credential username.")]
        [SampleUsage("john@openbots.com || {vCredentialUsername}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_CredentialUsername { get; set; }

        [DisplayName("Credential Password")]
        [Description("Enter the Credential password.")]
        [SampleUsage("john@openbots.com || {vCredentialPassword}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_CredentialPassword { get; set; }

        public UpdateCredentialCommand()
        {
            CommandName = "UpdateCredentialCommand";
            SelectionName = "Update Credential";
            CommandEnabled = true;
            CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vCredentialName = v_CredentialName.ConvertUserVariableToString(engine);
            var vCredentialUsername = v_CredentialUsername.ConvertUserVariableToString(engine);
            var vCredentialPassword = v_CredentialPassword.ConvertUserVariableToString(engine);

            var client = AuthMethods.GetAuthToken();
            var credential = CredentialMethods.GetCredential(client, $"name eq '{vCredentialName}'");

            if (credential == null)
                throw new Exception($"No Credential was found for '{vCredentialName}'");

            credential.UserName = vCredentialUsername;
            credential.PasswordSecret = vCredentialPassword;
            
            CredentialMethods.PutCredential(client, credential);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_CredentialName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_CredentialUsername", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultPasswordInputGroupFor("v_CredentialPassword", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" ['{v_CredentialName}']";
        }       
    }
}