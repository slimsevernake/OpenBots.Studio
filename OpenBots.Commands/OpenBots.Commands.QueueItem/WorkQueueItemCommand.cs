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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OpenBots.Commands.QueueItem
{
    [Serializable]
    [Group("QueueItem Commands")]
    [Description("This command gets and locks a QueueItem from an existing Queue in OpenBots Server.")]
    public class WorkQueueItemCommand : ScriptCommand
    {
        [PropertyDescription("Queue Name")]
        [InputSpecification("Enter the name of the Queue.")]
        [SampleUsage("Name || {vQueueName}")]
        [Remarks("QueueItem Text/Json values are store in the 'DataJson' key of a QueueItem Dictionary.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueName { get; set; }

        [PropertyDescription("Output QueueItem Dictionary Variable")]
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


            string agentSettingsPath = Environment.GetEnvironmentVariable("OpenBots_Agent_Config_Path", EnvironmentVariableTarget.Machine);

            if (agentSettingsPath == null)
                throw new Exception("Agent settings file not found");

            string agentSettingsText = File.ReadAllText(agentSettingsPath);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(agentSettingsText);
            string agentId = settings["AgentId"];

            if (string.IsNullOrEmpty(agentId))
                throw new Exception("Agent is not connected");

            Agent agent = AgentMethods.GetAgentById(client, Guid.Parse(agentId));

            Queue queue = QueueMethods.GetQueue(client, $"name eq '{vQueueName}'");
            var queueItem = QueueItemMethods.DequeueQueueItem(client, agent.Id, queue.Id);

            queueItemDict = queueItem.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                               .ToDictionary(prop => prop.Name, prop => prop.GetValue(queueItem, null));

            queueItemDict = queueItemDict.Where(kvp => kvp.Key == "LockTransactionKey" ||
                                                       kvp.Key == "Type" ||
                                                       kvp.Key == "JsonType" ||
                                                       kvp.Key == "DataJson" ||
                                                       kvp.Key == "LockedUntilUTC").ToDictionary(i =>i.Key, i => i.Value);

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
            return base.GetDisplayValue() + $" [From Queue '{v_QueueName}' - Store QueueItem Dictionary in '{v_OutputUserVariableName}']";
        }
    }
}