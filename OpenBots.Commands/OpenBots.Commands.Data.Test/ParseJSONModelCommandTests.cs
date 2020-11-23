using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OpenBots.Engine;
using OpenBots.Core.Utilities.CommonUtilities;
using System.Data;

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
            string selector = "$.rect.length";
            selector.StoreInUserVariable(_engine, "{selector}");

            DataTable selectorTable = new DataTable();
            selectorTable.Columns.Add("Json Selector");
            selectorTable.Columns.Add("Output Variable");
            DataRow row1 = selectorTable.NewRow();
            row1["Json Selector"] = "{selector}";
            row1["Output Variable"] = "{r1output}";

            _parseJSONModel.v_JsonObject = "{input}";
            _parseJSONModel.v_ParseObjects = selectorTable;

            _parseJSONModel.RunCommand(_engine);

            Assert.Equal("10", "{r1output}".ConvertUserVariableToString(_engine));
        }
    }
}
