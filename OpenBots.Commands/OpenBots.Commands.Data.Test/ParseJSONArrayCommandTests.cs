using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace OpenBots.Commands.Data.Test
{
    public class ParseJSONArrayCommandTests
    {
        private ParseJSONArrayCommand _parseJSONArray;
        private AutomationEngineInstance _engine;
        private readonly ITestOutputHelper output;

        public ParseJSONArrayCommandTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ParsesJSONArray()
        {
            _parseJSONArray = new ParseJSONArrayCommand();
            _engine = new AutomationEngineInstance(null);

            string jsonArray = "[\"val1\",\"val2\",\"val3\"]";
            string[] expectedResult = {"val1","val2","val3"};
            jsonArray.StoreInUserVariable(_engine, "{input}");

            _parseJSONArray.v_JsonArrayName = "{input}";
            _parseJSONArray.v_OutputUserVariableName = "{output}";

            _parseJSONArray.RunCommand(_engine);
            List<string> outputList = (List<string>)_parseJSONArray.v_OutputUserVariableName.ConvertUserVariableToObject(_engine);
            for (int i = 0;i < outputList.Count; i++)
            {
                Assert.Equal(expectedResult[i], outputList[i]);
            }
        }
    }
}
