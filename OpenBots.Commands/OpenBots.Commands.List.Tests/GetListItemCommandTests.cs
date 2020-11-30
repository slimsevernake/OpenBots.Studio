using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Collections.Generic;
using Xunit;
using System.Data;

namespace OpenBots.Commands.List.Tests
{
    public class GetListItemCommandTests
    {
        private AutomationEngineInstance _engine;
        private GetListItemCommand _getListItem;

        [Fact]
        public void GetsStringListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _getListItem = new GetListItemCommand();

            List<string> list = new List<string>();
            list.Add("item1");
            list.Add("item2");
            list.StoreInUserVariable(_engine, "{inputList}");

            _getListItem.v_ListName = "{inputList}";
            _getListItem.v_ItemIndex = "1";
            _getListItem.v_OutputUserVariableName = "{output}";

            _getListItem.RunCommand(_engine);

            Assert.Equal("item2", "{output}".ConvertUserVariableToString(_engine));
        }

        [Fact]
        public void GetsDataTableListItem()
        {
            _engine = new AutomationEngineInstance(null);
            _getListItem = new GetListItemCommand();

            List<DataTable> list = new List<DataTable>();
            DataTable item1 = new DataTable();
            item1.Columns.Add("d1col");
            DataTable item2 = new DataTable();
            item2.Columns.Add("d2col");
            list.Add(item1);
            list.Add(item2);
            list.StoreInUserVariable(_engine, "{inputList}");

            _getListItem.v_ListName = "{inputList}";
            _getListItem.v_ItemIndex = "1";
            _getListItem.v_OutputUserVariableName = "{output}";

            _getListItem.RunCommand(_engine);

            Assert.Equal(item2, (DataTable)"{output}".ConvertUserVariableToObject(_engine));
        }
    }
}
