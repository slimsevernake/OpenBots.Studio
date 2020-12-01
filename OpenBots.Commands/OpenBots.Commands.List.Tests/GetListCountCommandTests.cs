using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using Xunit;

namespace OpenBots.Commands.List.Tests
{
    public class GetListCountCommandTests
    {
        private AutomationEngineInstance _engine;
        private GetListCountCommand _getListCount;

        [Fact]
        public void GetsListCount()
        {
            _engine = new AutomationEngineInstance(null);
            _getListCount = new GetListCountCommand();

            List<string> inputList = new List<string>();
            inputList.Add("1");
            inputList.Add("2");
            inputList.Add("3");

            inputList.StoreInUserVariable(_engine, "{inputList}");

            _getListCount.v_ListName = "{inputList}";
            _getListCount.v_OutputUserVariableName = "{output}";

            _getListCount.RunCommand(_engine);

            Assert.Equal(inputList.Count, Int32.Parse("{output}".ConvertUserVariableToString(_engine)));
        }
    }
}
