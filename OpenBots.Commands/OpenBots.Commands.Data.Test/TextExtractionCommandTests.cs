using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Data;
using Xunit;

namespace OpenBots.Commands.Data.Test
{
    public class TextExtractionCommandTests
    {
        private AutomationEngineInstance _engine;
        private TextExtractionCommand _textExtraction;

        [Fact]
        public void ExtractsAllAfterText()
        {
            _engine = new AutomationEngineInstance(null);
            _textExtraction = new TextExtractionCommand();

            string input = "This is an example sentence";
            DataTable extractParams = new DataTable();
            extractParams.Columns.Add("Parameter Name");
            extractParams.Columns.Add("Parameter Value");
            DataRow row1 = extractParams.NewRow();
            row1["Parameter Name"] = "Leading Text";
            row1["Parameter Value"] = "{leadingText}";
            extractParams.Rows.Add(row1);
            DataRow row2 = extractParams.NewRow();
            row2["Parameter Name"] = "Skip Past Occurences";
            row2["Parameter Value"] = "0";
            extractParams.Rows.Add(row2);

            "This is an ".StoreInUserVariable(_engine, "{leadingText}");
            input.StoreInUserVariable(_engine, "{input}");

            _textExtraction.v_InputText = "{input}";
            _textExtraction.v_TextExtractionType = "Extract All After Text";
            _textExtraction.v_TextExtractionTable = extractParams;
            _textExtraction.v_OutputUserVariableName = "{output}";

            _textExtraction.RunCommand(_engine);

            Assert.Equal("example sentence", "{output}".ConvertUserVariableToString(_engine));
        }

        [Fact]
        public void ExtractsAllBeforeText()
        {
            _engine = new AutomationEngineInstance(null);
            _textExtraction = new TextExtractionCommand();

            string input = "This is an example sentence";
            DataTable extractParams = new DataTable();
            extractParams.Columns.Add("Parameter Name");
            extractParams.Columns.Add("Parameter Value");
            DataRow row1 = extractParams.NewRow();
            row1["Parameter Name"] = "Trailing Text";
            row1["Parameter Value"] = "{trailingText}";
            extractParams.Rows.Add(row1);
            DataRow row2 = extractParams.NewRow();
            row2["Parameter Name"] = "Skip Past Occurences";
            row2["Parameter Value"] = "0";
            extractParams.Rows.Add(row2);

            " an example sentence".StoreInUserVariable(_engine, "{trailingText}");
            input.StoreInUserVariable(_engine, "{input}");

            _textExtraction.v_InputText = "{input}";
            _textExtraction.v_TextExtractionType = "Extract All Before Text";
            _textExtraction.v_TextExtractionTable = extractParams;
            _textExtraction.v_OutputUserVariableName = "{output}";

            _textExtraction.RunCommand(_engine);

            Assert.Equal("This is", "{output}".ConvertUserVariableToString(_engine));
        }

        [Fact]
        public void ExtractsAllBetweenText()
        {
            _engine = new AutomationEngineInstance(null);
            _textExtraction = new TextExtractionCommand();

            string input = "This is an example sentence";
            DataTable extractParams = new DataTable();
            extractParams.Columns.Add("Parameter Name");
            extractParams.Columns.Add("Parameter Value");
            DataRow row1 = extractParams.NewRow();
            row1["Parameter Name"] = "Leading Text";
            row1["Parameter Value"] = "{leadingText}";
            extractParams.Rows.Add(row1);
            DataRow row2 = extractParams.NewRow();
            row2["Parameter Name"] = "Trailing Text";
            row2["Parameter Value"] = "{trailingText}";
            extractParams.Rows.Add(row2);
            DataRow row3 = extractParams.NewRow();
            row3["Parameter Name"] = "Skip Past Occurences";
            row3["Parameter Value"] = "0";
            extractParams.Rows.Add(row3);

            "This is an ".StoreInUserVariable(_engine, "{leadingText}");
            " sentence".StoreInUserVariable(_engine, "{trailingText}");
            input.StoreInUserVariable(_engine, "{input}");

            _textExtraction.v_InputText = "{input}";
            _textExtraction.v_TextExtractionType = "Extract All Between Text";
            _textExtraction.v_TextExtractionTable = extractParams;
            _textExtraction.v_OutputUserVariableName = "{output}";

            _textExtraction.RunCommand(_engine);

            Assert.Equal("example", "{output}".ConvertUserVariableToString(_engine));
        }
    }
}
