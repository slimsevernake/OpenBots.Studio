using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.IO;
using Xunit;

namespace OpenBots.Commands.Data.Test
{
    public class GetPDFTextCommandTests
    {
        private GetPDFTextCommand _getPDFText;
        private AutomationEngineInstance _engine;

        [Theory]
        [InlineData("File Path")]
        [InlineData("File URL")]
        public void GetsPDFText(string filePathOrUrl)
        {
            _getPDFText = new GetPDFTextCommand();
            _engine = new AutomationEngineInstance(null);
            string filepath = "";
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            if (filePathOrUrl.Equals("File Path"))
            {
                filepath = Path.Combine(projectDirectory, @"Resources\dummy.pdf");
            }
            else
            {
                filepath = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf";
            }
            filepath.StoreInUserVariable(_engine, "{filepath}");

            _getPDFText.v_FileSourceType = filePathOrUrl;
            _getPDFText.v_FilePath = "{filepath}";
            _getPDFText.v_OutputUserVariableName = "{outputText}";

            _getPDFText.RunCommand(_engine);

            Assert.Equal("Dummy PDF file", "{outputText}".ConvertUserVariableToString(_engine));
        }

        [Fact]
        public void HandlesInvalidFilepath()
        {
            _getPDFText = new GetPDFTextCommand();
            _engine = new AutomationEngineInstance(null);
            string filepath = "";

            filepath.StoreInUserVariable(_engine, "{filepath}");

            _getPDFText.v_FileSourceType = "File Path";
            _getPDFText.v_FilePath = "{filepath}";
            _getPDFText.v_OutputUserVariableName = "{outputText}";

            Assert.Throws<FileNotFoundException>(() => _getPDFText.RunCommand(_engine));
        }
    }
}
