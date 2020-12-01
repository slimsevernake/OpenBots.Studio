using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;
using Xunit.Abstractions;

namespace OpenBots.Commands.List.Tests
{
    public class AddListItemCommandTests
    {
        private AutomationEngineInstance _engine;
        private AddListItemCommand _addListItem;
        private readonly ITestOutputHelper output;

        public AddListItemCommandTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AddsStringListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _addListItem = new AddListItemCommand();

            List<string> stringList = new List<string>();
            string itemToAdd = "item1";

            stringList.StoreInUserVariable(_engine, "{list}");
            itemToAdd.StoreInUserVariable(_engine, "{itemToAdd}");

            _addListItem.v_ListName = "{list}";
            _addListItem.v_ListItem = "{itemToAdd}";

            _addListItem.RunCommand(_engine);

            List<string> outputList = (List<string>)"{list}".ConvertUserVariableToObject(_engine);
            Assert.Equal(itemToAdd, outputList[0]);
        }

        [Fact]
        public void AddsDataTableListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _addListItem = new AddListItemCommand();

            List<DataTable> stringList = new List<DataTable>();
            DataTable itemToAdd = new DataTable();
            itemToAdd.Columns.Add("first column");

            stringList.StoreInUserVariable(_engine, "{list}");
            itemToAdd.StoreInUserVariable(_engine, "{itemToAdd}");

            _addListItem.v_ListName = "{list}";
            _addListItem.v_ListItem = "{itemToAdd}";

            _addListItem.RunCommand(_engine);

            List<DataTable> outputList = (List<DataTable>)"{list}".ConvertUserVariableToObject(_engine);
            Assert.Equal(itemToAdd, outputList[0]);
        }

        [Fact]
        public void HandlesInvalidListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _addListItem = new AddListItemCommand();

            List<DataTable> stringList = new List<DataTable>();
            string itemToAdd = "newitem";

            stringList.StoreInUserVariable(_engine, "{list}");
            itemToAdd.StoreInUserVariable(_engine, "{itemToAdd}");

            _addListItem.v_ListName = "{list}";
            _addListItem.v_ListItem = "{itemToAdd}";

            Assert.Throws<Exception>(() => _addListItem.RunCommand(_engine));
        }
    }
}
