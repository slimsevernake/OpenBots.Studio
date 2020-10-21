using Microsoft.Office.Interop.Excel;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace OpenBots.Commands.Excel
{
    [Serializable]
    [Category("Excel Commands")]
    [Description("This command deletes a specific column in an Excel Worksheet.")]

    public class ExcelDeleteColumnCommand : ScriptCommand
    {
        [Required]
		[DisplayName("Excel Instance Name")]
        [Description("Enter the unique instance that was specified in the **Create Application** command.")]
        [SampleUsage("MyExcelInstance")]
        [Remarks("Failure to enter the correct instance or failure to first call the **Create Application** command will cause an error.")]
        public string v_InstanceName { get; set; }

        [Required]
		[DisplayName("Column Letter")]
        [Description("Enter the letter of the column to be deleted.")]
        [SampleUsage("A || {vColumnLetter}")]
        [Remarks("")]
        [Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
        public string v_ColumnLetter { get; set; }

        [Required]
		[DisplayName("Shift Cells Left")]
        [PropertyUISelectionOption("Yes")]
        [PropertyUISelectionOption("No")]
        [Description("'Yes' removes the entire column. 'No' only clears the column of its cell values.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_ShiftLeft { get; set; }

        public ExcelDeleteColumnCommand()
        {
            CommandName = "ExcelDeleteColumnCommand";
            SelectionName = "Delete Column";
            CommandEnabled = true;
            
            v_InstanceName = "DefaultExcel";
            v_ShiftLeft = "Yes";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var excelObject = v_InstanceName.GetAppInstance(engine);
            var excelInstance = (Application)excelObject;
            Worksheet workSheet = excelInstance.ActiveSheet;
            string vColumnToDelete = v_ColumnLetter.ConvertUserVariableToString(engine);

            var cells = workSheet.Range[vColumnToDelete + "1", Type.Missing];
            var entireColumn = cells.EntireColumn;
            if (v_ShiftLeft == "Yes")
                entireColumn.Delete();
            else
                entireColumn.Clear();
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InstanceName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_ColumnLetter", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_ShiftLeft", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [Delete Column '{v_ColumnLetter}' - Instance Name '{v_InstanceName}']";
        }
    }
}