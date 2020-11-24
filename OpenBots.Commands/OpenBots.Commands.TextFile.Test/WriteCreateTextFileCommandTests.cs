using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.IO;
using Xunit;

namespace OpenBots.Commands.TextFile.Test
{
    public class WriteCreateTextFileCommandTests
    {

        private WriteCreateTextFileCommand _writeCreateTextFile;
        private AutomationEngineInstance _engine;

        [Theory]
        [InlineData("Append")]
        [InlineData("Overwrite")]
        public void WritesToFile(string writeType)
        {
            _writeCreateTextFile = new WriteCreateTextFileCommand();
            _engine = new AutomationEngineInstance(null);

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string filePath = Path.Combine(projectDirectory, @"Resources\writetest.txt");
            filePath.StoreInUserVariable(_engine, "{filepath}");

            string testText = "Write this text to file";
            testText.StoreInUserVariable(_engine, "{testText}");

            _writeCreateTextFile.v_FilePath = "{filepath}";
            _writeCreateTextFile.v_TextToWrite = "{testText}";
            _writeCreateTextFile.v_Overwrite = writeType;

            string currentText = File.ReadAllText(filePath);

            _writeCreateTextFile.RunCommand(_engine);

            try
            {
                string textInFile =  File.ReadAllText(filePath);
                if(writeType == "Append")
                {
                    Assert.Equal(textInFile, currentText + testText);
                }
                else if(writeType == "Overwrite")
                {
                    Assert.Equal(textInFile, testText);
                }
               
            }
            catch (Exception ex)
            {
                throw(ex);
            }
            //Clean up test file for the next run
            File.WriteAllText(filePath, currentText);
        }
    }
}
