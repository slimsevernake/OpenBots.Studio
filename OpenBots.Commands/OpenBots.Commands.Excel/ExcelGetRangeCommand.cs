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
using System.Data;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Excel.Application;
using DataTable = System.Data.DataTable;

namespace OpenBots.Commands.Excel
{
	[Serializable]
	[Category("Excel Commands")]
	[Description("This command gets the range from an Excel Worksheet and stores it in a DataTable.")]
	public class ExcelGetRangeCommand : ScriptCommand
	{

		[Required]
		[DisplayName("Excel Instance Name")]
		[Description("Enter the unique instance that was specified in the **Create Application** command.")]
		[SampleUsage("MyExcelInstance")]
		[Remarks("Failure to enter the correct instance or failure to first call the **Create Application** command will cause an error.")]
		public string v_InstanceName { get; set; }

		[Required]
		[DisplayName("Range")]
		[Description("Enter the location of the range to extract.")]
		[SampleUsage("A1:B10 || A1: || {vRange} || {vStart}:{vEnd} || {vStart}:")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Range { get; set; }   

		[Required]
		[DisplayName("Add Headers")]
		[PropertyUISelectionOption("Yes")]
		[PropertyUISelectionOption("No")]
		[Description("When selected, the column headers from the specified spreadsheet range are also extracted.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_AddHeaders { get; set; }

		[Required]
		[DisplayName("Read Formulas")]
		[PropertyUISelectionOption("Yes")]
		[PropertyUISelectionOption("No")]
		[Description("When selected, formulas will be extracted rather than their calculated values.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_Formulas { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output DataTable Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public ExcelGetRangeCommand()
		{
			CommandName = "ExcelGetRangeCommand";
			SelectionName = "Get Range";
			CommandEnabled = true;
			
			v_InstanceName = "DefaultExcel";
			v_AddHeaders = "Yes";
			v_Formulas = "No";
			v_Range = "A1:";
		}

		public override void RunCommand(object sender)
		{         
			var engine = (AutomationEngineInstance)sender;
			var excelObject = v_InstanceName.GetAppInstance(engine);
			var vRange = v_Range.ConvertUserVariableToString(engine);
			var excelInstance = (Application)excelObject;

			Worksheet excelSheet = excelInstance.ActiveSheet;
			//Extract a range of cells
			var splitRange = vRange.Split(':');
			Range cellRange;
			Range sourceRange = excelSheet.UsedRange;
			

			//Attempt to extract a single cell

			if (splitRange[1] == "")
            {
				var cell = GetLastIndexOfNonEmptyCell(excelInstance, sourceRange, sourceRange.Range["A1"]);
				if (cell == "")
					throw new Exception("No data found in sheet.");
				cellRange = excelSheet.Range[splitRange[0], cell];
			}
			else
            {
				cellRange = excelSheet.Range[splitRange[0], splitRange[1]];
			}

			int rw = cellRange.Rows.Count;
			int cl = cellRange.Columns.Count;

			object[,] rangeData;
			if (v_Formulas == "Yes")
				rangeData = cellRange.Formula;
			else
				rangeData = cellRange.Value;

			int rCnt, cCnt;
			DataTable DT = new DataTable();

			int startRow;
			if (v_AddHeaders == "Yes")
				startRow = 2;
			else
				startRow = 1;

			for (rCnt = startRow; rCnt <= rw; rCnt++)
			{
				DataRow newRow = DT.NewRow();
				for (cCnt = 1; cCnt <= cl; cCnt++)
				{
					string colName = $"Column{cCnt - 1}";
					if (!DT.Columns.Contains(colName))
						DT.Columns.Add(colName);

					var cellValue = rangeData[rCnt, cCnt];

					if (cellValue == null)
						newRow[colName] = "";
					else
						newRow[colName] = cellValue.ToString();
				}
				DT.Rows.Add(newRow);
			}

			if (v_AddHeaders == "Yes")
			{
				//Set column names
				for (cCnt = 1; cCnt <= cl; cCnt++)
				{
					var cellValue = rangeData[1, cCnt];
					if (cellValue != null)
						DT.Columns[cCnt - 1].ColumnName = cellValue.ToString();
				}
			}

			DT.StoreInUserVariable(engine, v_OutputUserVariableName);           
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InstanceName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Range", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_AddHeaders", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_Formulas", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

			return RenderedControls;
		}

		private static string GetLastIndexOfNonEmptyCell(Application app, Range sourceRange, Range startPoint)
		{
			Range rng = sourceRange.Cells.Find(
				What: "*",
				After: startPoint,
				LookIn: XlFindLookIn.xlFormulas,
				LookAt: XlLookAt.xlPart,
				SearchDirection: XlSearchDirection.xlPrevious,
				MatchCase: false);
			if (rng == null)
				return "";
			return rng.Address[false, false, XlReferenceStyle.xlA1, Type.Missing, Type.Missing];
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Get Range '{v_Range}' - Store DataTable in '{v_OutputUserVariableName}' - Instance Name '{v_InstanceName}']";
		}
	}
}