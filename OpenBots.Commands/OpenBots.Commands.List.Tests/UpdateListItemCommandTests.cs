using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace OpenBots.Commands.List.Tests
{
    public class UpdateListItemCommandTests
    {
        private AutomationEngineInstance _engine;
        private UpdateListItemCommand _updateListItem;

        [Fact]
        public void UpdatesStringListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _updateListItem = new UpdateListItemCommand();

            List<string> inputList = new List<string>();
            inputList.Add("item1");
            inputList.Add("item2");
            string index = "0";
            string item = "item3";

            inputList.StoreInUserVariable(_engine, "{inputList}");
            index.StoreInUserVariable(_engine, "{index}");
            item.StoreInUserVariable(_engine, "{item}");

            _updateListItem.v_ListName = "{inputList}";
            _updateListItem.v_ListIndex = "{index}";
            _updateListItem.v_ListItem = "{item}";

            _updateListItem.RunCommand(_engine);

            List<string> outputList = (List<string>)"{inputList}".ConvertUserVariableToObject(_engine);
            Assert.Equal("item3", outputList[0]);
        }

        [Fact]
        public void UpdatesDataTableListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _updateListItem = new UpdateListItemCommand();

            List<DataTable> inputList = new List<DataTable>();
            DataTable item1 = new DataTable();
            item1.Columns.Add("d1col");
            DataTable item2 = new DataTable();
            item2.Columns.Add("d2col");
            inputList.Add(item1);
            inputList.Add(item2);
            string index = "0";
            DataTable newitem = new DataTable();
            newitem.Columns.Add("d3col");

            inputList.StoreInUserVariable(_engine, "{inputList}");
            index.StoreInUserVariable(_engine, "{index}");
            newitem.StoreInUserVariable(_engine, "{newitem}");

            _updateListItem.v_ListName = "{inputList}";
            _updateListItem.v_ListIndex = "{index}";
            _updateListItem.v_ListItem = "{newitem}";

            _updateListItem.RunCommand(_engine);

            List<DataTable> outputList = (List<DataTable>)"{inputList}".ConvertUserVariableToObject(_engine);
            Assert.Equal(newitem, outputList[0]);
        }

        [Fact]
        public void HandlesInvalidListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _updateListItem = new UpdateListItemCommand();

            List<DataTable> inputList = new List<DataTable>();
            DataTable item1 = new DataTable();
            string newItem = "item2";
            inputList.Add(item1);
            string index = "0";

            inputList.StoreInUserVariable(_engine, "{inputList}");
            index.StoreInUserVariable(_engine, "{index}");
            newItem.StoreInUserVariable(_engine, "{item}");

            _updateListItem.v_ListName = "{inputList}";
            _updateListItem.v_ListIndex = "{index}";
            _updateListItem.v_ListItem = "{item}";

            Assert.Throws<Exception>(() => _updateListItem.RunCommand(_engine));
        }
    }
}
