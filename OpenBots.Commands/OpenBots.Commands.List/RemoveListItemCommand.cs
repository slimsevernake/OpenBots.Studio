using Microsoft.Office.Interop.Outlook;
using MimeKit;
using System.ComponentModel;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Exception = System.Exception;

namespace OpenBots.Commands.List
{
    [Serializable]
    [Category("List Commands")]
    [Description("This command removes an item from an existing List variable at a specified index.")]
    public class RemoveListItemCommand : ScriptCommand
    {
        [DisplayName("List")]
        [Description("Provide a List variable.")]
        [SampleUsage("{vList}")]
        [Remarks("Any type of variable other than List will cause error.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_ListName { get; set; }

        [DisplayName("List Index")]
        [Description("Enter the List index where the item will be removed")]
        [SampleUsage("0 || {vIndex}")]
        [Remarks("Providing an out of range index will produce an exception.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_ListIndex { get; set; }

        public RemoveListItemCommand()
        {
            CommandName = "RemoveListItemCommand";
            SelectionName = "Remove List Item";
            CommandEnabled = true;           
        }

        public override void RunCommand(object sender)
        {
            //get sending instance
            var engine = (AutomationEngineInstance)sender;

            var vListVariable = v_ListName.ConvertUserVariableToObject(engine);
            var vListIndex = int.Parse(v_ListIndex.ConvertUserVariableToString(engine));

            if (vListVariable != null)
            {
                if (vListVariable is List<string>)
                    ((List<string>)vListVariable).RemoveAt(vListIndex);
                else if (vListVariable is List<DataTable>)
                    ((List<DataTable>)vListVariable).RemoveAt(vListIndex);
                else if (vListVariable is List<MailItem>)
                    ((List<MailItem>)vListVariable).RemoveAt(vListIndex);
                else if (vListVariable is List<MimeMessage>)
                    ((List<MimeMessage>)vListVariable).RemoveAt(vListIndex);
                else if (vListVariable is List<IWebElement>)
                    ((List<IWebElement>)vListVariable).RemoveAt(vListIndex);
                else
                    throw new Exception("Complex Variable List Type<T> Not Supported");
            }
            else
                throw new Exception("Attempted to write data to a variable, but the variable was not found. Enclose variables within braces, ex. {vVariable}");
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_ListName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_ListIndex", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Remove Item from List '{v_ListName}' at Index '{v_ListIndex}']";
        }
    }
}