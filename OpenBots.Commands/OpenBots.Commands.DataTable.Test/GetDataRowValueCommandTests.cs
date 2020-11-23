using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Data;
using Xunit;
using Data = System.Data;

namespace OpenBots.Commands.DataTable.Test
{
    public class GetDataRowValueCommandTests
    {
        private GetDataRowValueCommand _getDataRowValue;
        private AutomationEngineInstance _engine;

        [Theory]
        [InlineData("Column Name")]
        [InlineData("Column Index")]
        public void getsDataRowValue(string option)
        {
            _getDataRowValue = new GetDataRowValueCommand();
            _engine = new AutomationEngineInstance(null);

            Data.DataTable inputTable = new Data.DataTable();
            inputTable.Columns.Add("col1");
            DataRow row = inputTable.NewRow();
            row["col1"] = "data11";
            inputTable.Rows.Add(row);

            row.StoreInUserVariable(_engine, "{inputRow}");

            _getDataRowValue.v_DataRow = "{inputRow}";
            _getDataRowValue.v_Option = option;
            if (option == "Column Name") { 
                _getDataRowValue.v_DataValueIndex = "col1";
            }
            else
            {
                _getDataRowValue.v_DataValueIndex = "0";
            }
            _getDataRowValue.v_OutputUserVariableName = "{outputValue}";

            _getDataRowValue.RunCommand(_engine);

            string outputValue = _getDataRowValue.v_OutputUserVariableName.ConvertUserVariableToString(_engine);
            Assert.Equal(inputTable.Rows[0]["col1"], outputValue);
        }
    }
}
