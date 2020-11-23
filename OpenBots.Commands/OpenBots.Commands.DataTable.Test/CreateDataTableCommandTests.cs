using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Data;
using Xunit;
using Data = System.Data;

namespace OpenBots.Commands.DataTable.Test
{
    public class CreateDataTableCommandTests
    {
        private CreateDataTableCommand _createDataTableCommand;
        private AutomationEngineInstance _engine;

        [Fact]
        public void CreatesDataTable()
        {
            _createDataTableCommand = new CreateDataTableCommand();
            _engine = new AutomationEngineInstance(null);

            Data.DataTable columnNameDataTable = new Data.DataTable
            {
                TableName = "ColumnNamesDataTable" + DateTime.Now.ToString("MMddyy.hhmmss")
            };
            "Col1".StoreInUserVariable(_engine, "{Col1}");
            columnNameDataTable.Columns.Add("{Col1}");

            _createDataTableCommand.v_ColumnNameDataTable = columnNameDataTable;
            _createDataTableCommand.v_OutputUserVariableName = "{outputTable}";

            _createDataTableCommand.RunCommand(_engine);

            Data.DataTable expectedDt = new Data.DataTable();
            foreach (DataRow rwColumnName in columnNameDataTable.Rows)
            {
                expectedDt.Columns.Add(rwColumnName.Field<string>("Column Name").ConvertUserVariableToString(_engine));
            }

            Data.DataTable resultDataTable = (Data.DataTable)_createDataTableCommand.v_OutputUserVariableName.ConvertUserVariableToObject(_engine);

            for (int row = 0; row < expectedDt.Rows.Count; row++)
            {
                for (int col = 0; col < expectedDt.Columns.Count; col++)
                {
                    Assert.Equal(expectedDt.Rows[row][expectedDt.Columns[col]], resultDataTable.Rows[row][resultDataTable.Columns[col]]);
                }
            }
        }
    }
}
