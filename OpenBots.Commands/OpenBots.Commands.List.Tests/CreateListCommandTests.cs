using Microsoft.Office.Interop.Outlook;
using MimeKit;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;
using Xunit.Abstractions;

namespace OpenBots.Commands.List.Tests
{
    public class CreateListCommandTests
    {
        private AutomationEngineInstance _engine;
        private CreateListCommand _createList;
        private readonly ITestOutputHelper output;

        public CreateListCommandTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("String")]
        [InlineData("DataTable")]
        [InlineData("MailItem (Outlook)")]
        [InlineData("MimeMessage (IMAP/SMTP)")]
        [InlineData("IWebElement")]
        public void CreatesList(string listType)
        {
            _engine = new AutomationEngineInstance(null);
            _createList = new CreateListCommand();

            _createList.v_ListType = listType;
            _createList.v_OutputUserVariableName = "{output}";

            _createList.RunCommand(_engine);

            dynamic expectedList = new List<string>();
            switch (listType)
            {
                case "String":
                    expectedList = new List<string>();
                    break;
                case "DataTable":
                    expectedList = new List<DataTable>();
                    break;
                case "MailItem (Outlook)":
                    expectedList = new List<MailItem>();
                    break;
                case "MimeMessage (IMAP/SMTP)":
                    expectedList = new List<MimeMessage>();
                    break;
                case "IWebElement":
                    expectedList = new List<IWebElement>();
                    break;
                default:
                    break;
            }
            output.WriteLine("{output}".ConvertUserVariableToObject(_engine).GetType().ToString());
            output.WriteLine(expectedList.GetType().ToString());

            Assert.True(Object.ReferenceEquals("{output}".ConvertUserVariableToObject(_engine).GetType(), expectedList.GetType()));
        }

        [Fact]
        public void RejectsIncorrectValue()
        {
            _engine = new AutomationEngineInstance(null);
            _createList = new CreateListCommand();

            int item1 = 1;
            bool item2 = false;
            item1.StoreInUserVariable(_engine, "{item1}");
            item2.StoreInUserVariable(_engine, "{item2}");

            _createList.v_ListType = "DataTable";
            _createList.v_ListItems = "{item1},{item2}";
            _createList.v_OutputUserVariableName = "{output}";

            Assert.Throws<System.Exception>(() => _createList.RunCommand(_engine));
        }
    }
}
