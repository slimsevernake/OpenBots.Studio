using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Data;
using Xunit;
using Data = System.Data;

namespace OpenBots.Commands.DataTable.Test
{
    public class GetDataRowCountCommandTests
    {
        private GetDataRowCountCommand _getDataRowCount;
        private AutomationEngineInstance _engine;

        [Fact]
        public void getsDataRowCount()
        {
            _getDataRowCount = new GetDataRowCountCommand();
            _engine = new AutomationEngineInstance(null);

            // Set up existing data table for the command
            Data.DataTable inputTable = new Data.DataTable();
            inputTable.Columns.Add("Column1");
            DataRow row1 = inputTable.NewRow();
            row1["Column1"] = "data1";
            inputTable.Rows.Add(row1);
            inputTable.StoreInUserVariable(_engine, "{inputTable}");

            _getDataRowCount.v_DataTable = "{inputTable}";
            _getDataRowCount.v_OutputUserVariableName = "{outputCount}";

            _getDataRowCount.RunCommand(_engine);

            Assert.Equal("1", (string)_getDataRowCount.v_OutputUserVariableName.ConvertUserVariableToString(_engine));
        }
    }
}
