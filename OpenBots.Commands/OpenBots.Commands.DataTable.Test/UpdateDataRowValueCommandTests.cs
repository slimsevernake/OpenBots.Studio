using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Data;
using Xunit;
using Data = System.Data;

namespace OpenBots.Commands.DataTable.Test
{
    public class UpdateDataRowValueCommandTests
    {
        private UpdateDataRowValueCommand _updateDataRowValue;
        private AutomationEngineInstance _engine;

        [Theory]
        [InlineData("Column Name", "firstname", "jane")]
        [InlineData("Column Index", "0", "")]
        public void updatesDataRowValue(string option, string searchVal, string updateVal)
        {
            _updateDataRowValue = new UpdateDataRowValueCommand();
            _engine = new AutomationEngineInstance(null);

            Data.DataTable inputTable = new Data.DataTable();
            inputTable.Columns.Add("firstname");
            DataRow row = inputTable.NewRow();
            row["firstname"] = "john";
            inputTable.Rows.Add(row);
            row.StoreInUserVariable(_engine, "{inputRow}");

            _updateDataRowValue.v_DataRow = "{inputRow}";
            _updateDataRowValue.v_Option = option;
            _updateDataRowValue.v_DataValueIndex = searchVal;
            _updateDataRowValue.v_DataRowValue = updateVal;

            _updateDataRowValue.RunCommand(_engine);
            DataRow outputRow = (DataRow)_updateDataRowValue.v_DataRow.ConvertUserVariableToObject(_engine);
            Assert.Equal(updateVal, outputRow["firstname"]);
        }
    }
}
