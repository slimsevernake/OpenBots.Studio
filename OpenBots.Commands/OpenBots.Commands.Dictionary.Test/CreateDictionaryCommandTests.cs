using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace OpenBots.Commands.Dictionary.Test
{
    public class CreateDictionaryCommandTests
    {
        private CreateDictionaryCommand _createDictionary;
        private AutomationEngineInstance _engine;

        [Fact]
        public void CreatesDictionary()
        {
            _createDictionary = new CreateDictionaryCommand();
            _engine = new AutomationEngineInstance(null);
            DataTable inputDt = new DataTable();
            inputDt.Columns.Add("Keys");
            inputDt.Columns.Add("Values");
            DataRow row1 = inputDt.NewRow();
            row1["Keys"] = "key1";
            row1["Values"] = "val1";
            inputDt.Rows.Add(row1);
            inputDt.StoreInUserVariable(_engine, "{inputDt}");

            _createDictionary.v_ColumnNameDataTable = (DataTable)"{inputDt}".ConvertUserVariableToObject(_engine);
            _createDictionary.v_OutputUserVariableName = "{output}";

            _createDictionary.RunCommand(_engine);

            Dictionary<string, string> outDict = (Dictionary<string, string>)"{output}".ConvertUserVariableToObject(_engine);

            Assert.True(outDict.ContainsKey("key1"));
            Assert.Equal("val1", outDict["key1"]);
        }
    }
}
