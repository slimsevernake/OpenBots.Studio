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

namespace OpenBots.Commands.QueueItem
{
    [Serializable]
    [Group("QueueItem Commands")]
    [Description("This command extends a QueueItem in an existing Queue in OpenBots Server.")]
    public class ExtendQueueItemCommand : ScriptCommand
    {
        [PropertyDescription("QueueItem")]
        [InputSpecification("Enter a QueueItem Dictionary variable.")]
        [SampleUsage("{vQueueItem}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_QueueItem { get; set; }

        public ExtendQueueItemCommand()
        {
            CommandName = "ExtendQueueItemCommand";
            SelectionName = "Extend QueueItem";
            CommandEnabled = true;
            CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vQueueItem = (Dictionary<string, object>)v_QueueItem.ConvertUserVariableToObject(engine);

            var client = AuthMethods.GetAuthToken();

            Guid transactionKey = (Guid)vQueueItem["LockTransactionKey"];
            QueueItemMethods.ExtendQueueItem(client, transactionKey);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_QueueItem", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [QueueItem '{v_QueueItem}']";
        }
    }
}