using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using Xunit;

namespace OpenBots.Commands.Data.Test
{
    public class GetWordCountCommandTests
    {
        private GetWordCountCommand _getWordCount;
        private AutomationEngineInstance _engine;

        [Fact]
        public void GetsWordCount()
        {
            _getWordCount = new GetWordCountCommand();
            _engine = new AutomationEngineInstance(null);

            string input = "Test input sentence";
            input.StoreInUserVariable(_engine, "{input}");

            _getWordCount.v_InputValue = "{input}";
            _getWordCount.v_OutputUserVariableName = "{output}";

            _getWordCount.RunCommand(_engine);

            Assert.Equal(3, Int32.Parse(_getWordCount.v_OutputUserVariableName.ConvertUserVariableToString(_engine)));
        }
    }
}
