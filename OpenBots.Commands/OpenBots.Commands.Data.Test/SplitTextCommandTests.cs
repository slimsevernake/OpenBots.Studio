using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Collections.Generic;
using Xunit;

namespace OpenBots.Commands.Data.Test
{
    public class SplitTextCommandTests
    {
        private SplitTextCommand _splitText;
        private AutomationEngineInstance _engine;

        [Fact]
        public void SplitsText()
        {
            _splitText = new SplitTextCommand();
            _engine = new AutomationEngineInstance(null);

            string inputText = "test text";
            string splitCharacter = " ";
            inputText.StoreInUserVariable(_engine, "{input}");
            splitCharacter.StoreInUserVariable(_engine, "{splitChar}");

            _splitText.v_InputText = "{input}";
            _splitText.v_SplitCharacter = "{splitChar}";
            _splitText.v_OutputUserVariableName = "{output}";

            _splitText.RunCommand(_engine);

            List<string> splitText = (List<string>)"{output}".ConvertUserVariableToObject(_engine);
            Assert.Equal("test", splitText[0]);
            Assert.Equal("text", splitText[1]);
        }

        [Fact]
        public void SplitsTextWithMultipleDelimiters()
        {
            _splitText = new SplitTextCommand();
            _engine = new AutomationEngineInstance(null);

            string inputText = "test text:with!multiple;delimiters";
            List<string> splitCharacters = new List<string>();
            splitCharacters.Add(" ");
            splitCharacters.Add(":");
            splitCharacters.Add("!");
            splitCharacters.Add(";");
            inputText.StoreInUserVariable(_engine, "{input}");
            splitCharacters.StoreInUserVariable(_engine, "{splitChar}");

            _splitText.v_InputText = "{input}";
            _splitText.v_SplitCharacter = "{splitChar}";
            _splitText.v_OutputUserVariableName = "{output}";

            _splitText.RunCommand(_engine);

            List<string> splitText = (List<string>)"{output}".ConvertUserVariableToObject(_engine);
            Assert.Equal("test", splitText[0]);
            Assert.Equal("text", splitText[1]);
            Assert.Equal("with", splitText[2]);
            Assert.Equal("multiple", splitText[3]);
            Assert.Equal("delimiters", splitText[4]);
        }
    }
}
