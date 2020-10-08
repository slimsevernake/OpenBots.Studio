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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OpenBots.Commands.QueueItem
{
    [Serializable]
    [Group("QueueItem Commands")]
    [Description("This command gets a QueueItem from an existing Queue in OpenBots Server.")]
    public class WorkQueueItemCommand : ScriptCommand
    {
        [PropertyDescription("Queue Name")]
        [InputSpecification("Enter the name of the Queue.")]
        [SampleUsage("Name || {vQueueName}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueName { get; set; }

        [PropertyDescription("Output QueueItem Variable")]
        [InputSpecification("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        public WorkQueueItemCommand()
        {
            CommandName = "WorkQueueItemCommand";
            SelectionName = "Work QueueItem";
            CommandEnabled = true;
            CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vQueueName = v_QueueName.ConvertUserVariableToString(engine);
            Dictionary<string, object> queueItemDict = new Dictionary<string, object>();

            var client = AuthMethods.GetAuthToken();

            Agent agent = AgentMethods.GetAgent(client, $"name eq 'AliAgent'"); //temporary

            Queue queue = QueueMethods.GetQueue(client, $"name eq '{vQueueName}'");
            var queueItem = QueueItemMethods.DequeueQueueItem(client, agent.Id, queue.Id); //TODO add agentID and test

            queueItemDict = queueItem.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                               .ToDictionary(prop => prop.Name, prop => prop.GetValue(queueItem, null));

            queueItemDict.StoreInUserVariable(engine, v_OutputUserVariableName);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_QueueName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_OutputUserVariableName", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [From Queue '{v_QueueName}' - Store QueueItem in '{v_OutputUserVariableName}']";
        }
    }
}