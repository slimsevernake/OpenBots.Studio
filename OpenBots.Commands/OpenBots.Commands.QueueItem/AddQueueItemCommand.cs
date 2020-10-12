using Newtonsoft.Json;
using OpenBots.Core.Attributes.ClassAttributes;
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
using System.Windows.Forms;
using QueueItemModel = OpenBots.Core.Server.Models.QueueItem;

namespace OpenBots.Commands.QueueItem
{
    [Serializable]
    [Group("QueueItem Commands")]
    [Description("This command adds a QueueItem to an existing Queue in OpenBots Server.")]
    public class AddQueueItemCommand : ScriptCommand
    {
        [PropertyDescription("Queue Name")]
        [InputSpecification("Enter the name of the existing Queue.")]
        [SampleUsage("Name || {vQueueName}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueName { get; set; }

        [PropertyDescription("QueueItem Name")]
        [InputSpecification("Enter the name of the new QueueItem.")]
        [SampleUsage("Name || {vQueueItemName}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueItemName { get; set; }
      
        [PropertyDescription("QueueItem Type")]
        [PropertyUISelectionOption("Text")]
        [PropertyUISelectionOption("Json")]
        [InputSpecification("Specify the type of the new QueueItem.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_QueueItemType { get; set; }

        [PropertyDescription("Json Type")]
        [InputSpecification("Specify the type of the Json.")]
        [SampleUsage("Company || {vJsonType}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_JsonType { get; set; }

        [PropertyDescription("QueueItem Value")]
        [InputSpecification("Enter the value of the new QueueItem.")]
        [SampleUsage("Value || {vQueueItemValue}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueItemTextValue { get; set; }

        [PropertyDescription("Priority")]
        [InputSpecification("Enter a priority value between 0-100.")]
        [SampleUsage("100 || {vPriority}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_Priority { get; set; }

        [JsonIgnore]
        [NonSerialized]
        public List<Control> JsonTypeControls;

        public AddQueueItemCommand()
        {
            CommandName = "AddQueueItemCommand";
            SelectionName = "Add QueueItem";
            CommandEnabled = true;
            CustomRendering = true;
            v_QueueItemType = "Text";
            v_Priority = "100";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vQueueName = v_QueueName.ConvertUserVariableToString(engine);
            var vQueueItemName = v_QueueItemName.ConvertUserVariableToString(engine);
            var vJsonType = v_JsonType.ConvertUserVariableToString(engine);            
            var vPriority = v_Priority.ConvertUserVariableToString(engine);
            var vQueueItemTextValue = v_QueueItemTextValue.ConvertUserVariableToString(engine);

            var client = AuthMethods.GetAuthToken();
            Queue queue = QueueMethods.GetQueue(client, $"name eq '{vQueueName}'");

            QueueItemModel queueItem = new QueueItemModel()
            {
                IsLocked = false,
                QueueId = queue.Id,
                Type = v_QueueItemType,
                JsonType = vJsonType,
                DataJson = vQueueItemTextValue,
                Name = vQueueItemName,
                IsDeleted = false,
                Priority = int.Parse(vPriority)
            };

            QueueItemMethods.EnqueQueueItem(client, queueItem);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_QueueName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_QueueItemName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_QueueItemType", this, editor));
            ((ComboBox)RenderedControls[7]).SelectedIndexChanged += QueueItemTypeComboBox_SelectedValueChanged;

            JsonTypeControls = new List<Control>();
            JsonTypeControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_JsonType", this, editor));
            foreach (var ctrl in JsonTypeControls)
                ctrl.Visible = false;
            RenderedControls.AddRange(JsonTypeControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_QueueItemTextValue", this, editor, 100, 300));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Priority", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" ['{v_QueueItemName}' of Type '{v_QueueItemType}' to Queue '{v_QueueName}']";
        }

        private void QueueItemTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((ComboBox)RenderedControls[7]).Text == "Json")
            {
                foreach (var ctrl in JsonTypeControls)
                    ctrl.Visible = true;
            }
            else
            {
                foreach (var ctrl in JsonTypeControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }
            }
        }
    }
}