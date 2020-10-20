using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

namespace OpenBots.Commands.QueueItem
{
    [Serializable]
    [Category("QueueItem Commands")]
    [Description("This command updates the status of a QueueItem in an existing Queue in OpenBots Server.")]
    public class SetQueueItemStatusCommand : ScriptCommand
    {
        [Required]
		[DisplayName("QueueItem")]
        [Description("Enter a QueueItem Dictionary variable.")]
        [SampleUsage("{vQueueItem}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueItem { get; set; }

        [Required]
		[DisplayName("QueueItem Status Type")]
        [PropertyUISelectionOption("Successful")]
        [PropertyUISelectionOption("Failed - Should Retry")]
        [PropertyUISelectionOption("Failed - Fatal")]
        [Description("Specify the QueueItem status type.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_QueueItemStatusType { get; set; }

		[DisplayName("QueueItem Error Code (Optional)")]
        [Description("Enter the QueueItem code.")]
        [SampleUsage("400 || {vStatusCode}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueItemErrorCode { get; set; }

		[DisplayName("QueueItem Error Message (Optional)")]
        [Description("Enter the QueueItem error message.")]
        [SampleUsage("File not found || {vStatusMessage}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueItemErrorMessage { get; set; }

        [JsonIgnore]
        private List<Control> _errorMessageControls;

        public SetQueueItemStatusCommand()
        {
            CommandName = "SetQueueItemStatusCommand";
            SelectionName = "Set QueueItem Status";
            CommandEnabled = true;
            
            v_QueueItemStatusType = "Successful";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vQueueItem = (Dictionary<string, object>)v_QueueItem.ConvertUserVariableToObject(engine);
            var vQueueItemErrorMessage = v_QueueItemErrorMessage.ConvertUserVariableToString(engine);
            var vQueueItemErrorCode = v_QueueItemErrorCode.ConvertUserVariableToString(engine);

            var client = AuthMethods.GetAuthToken();

            Guid transactionKey = (Guid)vQueueItem["LockTransactionKey"];

            switch (v_QueueItemStatusType)
            {
                case "Successful":
                    QueueItemMethods.CommitQueueItem(client, transactionKey);
                    break;
                case "Failed - Should Retry":
                    QueueItemMethods.RollbackQueueItem(client, transactionKey, vQueueItemErrorCode, vQueueItemErrorMessage, false);
                    break;
                case "Failed - Fatal":
                    QueueItemMethods.RollbackQueueItem(client, transactionKey, vQueueItemErrorCode, vQueueItemErrorMessage, true);
                    break;
            }
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_QueueItem", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_QueueItemStatusType", this, editor));
            ((ComboBox)RenderedControls[4]).SelectedIndexChanged += QueueItemStatusTypeComboBox_SelectedValueChanged;

            _errorMessageControls = new List<Control>();
            _errorMessageControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_QueueItemErrorCode", this, editor));
            _errorMessageControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_QueueItemErrorMessage", this, editor));
            foreach (var ctrl in _errorMessageControls)
                ctrl.Visible = false;
            RenderedControls.AddRange(_errorMessageControls);

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            if (v_QueueItemStatusType != "Successful")
                return base.GetDisplayValue() + $" [Set '{v_QueueItem}' Status to '{v_QueueItemStatusType}' With Message '{v_QueueItemErrorMessage}']";
            else
                return base.GetDisplayValue() + $" [Set '{v_QueueItem}' Status to '{v_QueueItemStatusType}']";
        }

        private void QueueItemStatusTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((ComboBox)RenderedControls[4]).Text != "Successful")
            {
                foreach (var ctrl in _errorMessageControls)
                    ctrl.Visible = true;
            }
            else
            {
                foreach (var ctrl in _errorMessageControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }
            }
        }
    }
}