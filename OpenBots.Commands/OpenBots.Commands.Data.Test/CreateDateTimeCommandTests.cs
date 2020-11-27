using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using Xunit;

namespace OpenBots.Commands.Data.Test
{
    public class CreateDateTimeCommandTests
    {
        private CreateDateTimeCommand _createDateTime;
        private AutomationEngineInstance _engine;

        [Fact]
        public void CreatesDateTime()
        {
            _createDateTime = new CreateDateTimeCommand();
            _engine = new AutomationEngineInstance(null);

            string year = "2020";
            string month = "jan";
            string day = "1";
            string time = "1:10";
            string AMorPM = "PM";

            year.StoreInUserVariable(_engine, "{year}");
            month.StoreInUserVariable(_engine, "{month}");
            day.StoreInUserVariable(_engine, "{day}");
            time.StoreInUserVariable(_engine, "{time}");

            _createDateTime.v_Year = "{year}";
            _createDateTime.v_Month = "{month}";
            _createDateTime.v_Day = "{day}";
            _createDateTime.v_Time = "{time}";
            _createDateTime.v_AMorPM = AMorPM;
            _createDateTime.v_OutputUserVariableName = "{outputVar}";

            _createDateTime.RunCommand(_engine);

            Object output = _createDateTime.v_OutputUserVariableName.ConvertUserVariableToObject(_engine);
            Assert.IsType<DateTime>(output);
            DateTime dateOutput = (DateTime)output;
            Assert.Equal(Int32.Parse(year), dateOutput.Year);
            Assert.Equal(1,dateOutput.Month);
            Assert.Equal(1, dateOutput.Day);
            Assert.Equal(13, dateOutput.Hour);
            Assert.Equal(10, dateOutput.Minute);
        }
    }
}
