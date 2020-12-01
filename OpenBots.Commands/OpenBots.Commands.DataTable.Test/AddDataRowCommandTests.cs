using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Data;
using Xunit;
using Xunit.Abstractions;
using Data = System.Data;

namespace OpenBots.Commands.DataTable.Test
{
    public class AddDataRowCommandTests
    {

        private AddDataRowCommand _addDataRow;
        private AutomationEngineInstance _engine;
        private readonly ITestOutputHelper output;

        public AddDataRowCommandTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void addsDataRow()
        {
            _addDataRow = new AddDataRowCommand();
            _engine = new AutomationEngineInstance(null);

            Data.DataTable inputTable = new Data.DataTable();
            inputTable.Columns.Add("firstname");
            inputTable.Columns.Add("lastname");
            DataRow inputrow = inputTable.NewRow();
            inputrow["firstname"] = "john";
            inputrow["lastname"] = "smith";
            inputTable.StoreInUserVariable(_engine, "{inputTable}");
            _addDataRow.v_DataTable = "{inputTable}";
            Data.DataRow newrow = _addDataRow.v_DataRowDataTable.NewRow();
            newrow["Column Name"] = "firstname";
            newrow["Data"] = "john";
            Data.DataRow newrow2 = _addDataRow.v_DataRowDataTable.NewRow();
            newrow2["Column Name"] = "lastname";
            newrow2["Data"] = "smith";

            _addDataRow.v_DataRowDataTable.Rows.Add(newrow);
            _addDataRow.v_DataRowDataTable.Rows.Add(newrow2);

            _addDataRow.RunCommand(_engine);

            Data.DataTable outputTable = (Data.DataTable)_addDataRow.v_DataTable.ConvertUserVariableToObject(_engine);
            Assert.Equal(inputTable.Rows[0]["firstname"], outputTable.Rows[0]["firstname"]);
            Assert.Equal(inputTable.Rows[0]["lastname"], outputTable.Rows[0]["lastname"]);
        }
    }
}
