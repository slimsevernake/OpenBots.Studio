using Microsoft.Office.Interop.Excel;
using System.ComponentModel;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace OpenBots.Commands.Excel
{
    [Serializable]
    [Category("Excel Commands")]
    [Description("This commands retrieves the index of the last row of a range in an Excel Worksheet.")]

    public class ExcelGetLastRowIndexCommand : ScriptCommand
    {
        [DisplayName("Excel Instance Name")]
        [Description("Enter the unique instance that was specified in the **Create Application** command.")]
        [SampleUsage("MyExcelInstance")]
        [Remarks("Failure to enter the correct instance or failure to first call the **Create Application** command will cause an error.")]
        public string v_InstanceName { get; set; }

        [DisplayName("Column")]
        [Description("Enter the letter of the column to check.")]
        [SampleUsage("A || {vColumnLetter}")]
        [Remarks("")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_ColumnLetter { get; set; }

        [DisplayName("Output Last Row Index Variable")]
        [Description("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        public ExcelGetLastRowIndexCommand()
        {
            CommandName = "ExcelGetLastRowIndexCommand";
            SelectionName = "Get Last Row Index";
            CommandEnabled = true;
            
            v_InstanceName = "DefaultExcel";
            v_ColumnLetter = "A";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vColumnLetter = v_ColumnLetter.ConvertUserVariableToString(engine);
            var excelObject = v_InstanceName.GetAppInstance(engine);

            var excelInstance = (Application)excelObject;
            var excelSheet = excelInstance.ActiveSheet;
            var lastRow = (int)excelSheet.Cells(excelSheet.Rows.Count, vColumnLetter).End(XlDirection.xlUp).Row;

            lastRow.ToString().StoreInUserVariable(engine, v_OutputUserVariableName);   
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InstanceName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_ColumnLetter", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [For Column '{v_ColumnLetter}' - Store Last Row Index in '{v_OutputUserVariableName}' - Instance Name '{v_InstanceName}']";
        }
    }
}