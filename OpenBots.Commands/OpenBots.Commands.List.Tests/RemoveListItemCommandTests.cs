using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Collections.Generic;
using Xunit;
using System.Data;

namespace OpenBots.Commands.List.Tests
{
    public class RemoveListItemCommandTests
    {
        private AutomationEngineInstance _engine;
        private RemoveListItemCommand _removeListItem;

        [Fact]
        public void RemovesStringListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _removeListItem = new RemoveListItemCommand();

            List<string> inputList = new List<string>();
            inputList.Add("item1");
            inputList.Add("item2");
            string index = "0";

            inputList.StoreInUserVariable(_engine, "{inputList}");
            index.StoreInUserVariable(_engine, "{index}");

            _removeListItem.v_ListName = "{inputList}";
            _removeListItem.v_ListIndex = "{index}";

            _removeListItem.RunCommand(_engine);
            List<string> outputList = (List<string>)"{inputList}".ConvertUserVariableToObject(_engine);
            Assert.Equal("item2", outputList[0]);
        }

        [Fact]
        public void RemovesDataTableListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _removeListItem = new RemoveListItemCommand();

            List<DataTable> inputList = new List<DataTable>();
            DataTable item1 = new DataTable();
            item1.Columns.Add("d1col");
            DataTable item2 = new DataTable();
            item2.Columns.Add("d2col");
            inputList.Add(item1);
            inputList.Add(item2);
            string index = "0";

            inputList.StoreInUserVariable(_engine, "{inputList}");
            index.StoreInUserVariable(_engine, "{index}");

            _removeListItem.v_ListName = "{inputList}";
            _removeListItem.v_ListIndex = "{index}";

            _removeListItem.RunCommand(_engine);
            List<DataTable> outputList = (List<DataTable>)"{inputList}".ConvertUserVariableToObject(_engine);
            Assert.Equal(item2, outputList[0]);
        }
    }
}
