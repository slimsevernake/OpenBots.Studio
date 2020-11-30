using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace OpenBots.Commands.Data.Test
{
    public class ParseJSONModelCommandTest
    {
        private ParseJSONModelCommand _parseJSONModel;
        private AutomationEngineInstance _engine;

        [Fact]
        public void ParsesJSONModel()
        {
            _parseJSONModel = new ParseJSONModelCommand();
            _engine = new AutomationEngineInstance(null);

            string jsonObject = "{\"rect\":{\"length\":10, \"width\":5}}";
            jsonObject.StoreInUserVariable(_engine, "{input}");
            string selector = "rect.length";
            selector.StoreInUserVariable(_engine, "{selector}");

            DataTable selectorTable = new DataTable();
            selectorTable.Columns.Add("Json Selector");
            selectorTable.Columns.Add("Output Variable");
            DataRow row1 = selectorTable.NewRow();
            row1["Json Selector"] = "{selector}";
            row1["Output Variable"] = "{r1output}";
            selectorTable.Rows.Add(row1);

            _parseJSONModel.v_JsonObject = "{input}";
            _parseJSONModel.v_ParseObjects = selectorTable;

            _parseJSONModel.RunCommand(_engine);
            List<string> resultList = (List<string>)"{r1output}".ConvertUserVariableToObject(_engine);
            Assert.Equal("10", resultList[0]);
        }
    }
}
