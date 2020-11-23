using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using Xunit;

namespace OpenBots.Commands.Data.Test
{
    public class SubstringCommandTests
    {
        private AutomationEngineInstance _engine;
        private SubstringCommand _substringCommand;

        [Fact]
        public void CreatesSubstring()
        {
            _engine = new AutomationEngineInstance(null);
            _substringCommand = new SubstringCommand();

            string input = "test text";
            string startIndex = "5";
            string length = "4";
            input.StoreInUserVariable(_engine, "{input}");
            startIndex.StoreInUserVariable(_engine, "{start}");
            length.StoreInUserVariable(_engine, "{length}");

            _substringCommand.v_InputText = "{input}";
            _substringCommand.v_StartIndex = "{start}";
            _substringCommand.v_StringLength = "{length}";
            _substringCommand.v_OutputUserVariableName = "{output}";

            _substringCommand.RunCommand(_engine);

            Assert.Equal("text", "{output}".ConvertUserVariableToString(_engine));
        }
    }
}
