using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace OpenBots.Commands.Data.Test
{
    public class DateCalculationCommandTests
    {
        private DateCalculationCommand _dateCalculation;
        private AutomationEngineInstance _engine;

        [Theory]
        [ClassData(typeof(DateTestData))]
        public void CorrectlyPerformsOperation(DateTime input, string calcMethod, string increment, dynamic expectedResult)
        {
            _dateCalculation = new DateCalculationCommand();
            _engine = new AutomationEngineInstance(null);
            string defaultFormat = "MM/dd/yyyy hh:mm:ss";
            if (calcMethod.Contains("Get"))
            {
                defaultFormat = null;
            }
            defaultFormat.StoreInUserVariable(_engine, "{format}");
            increment.StoreInUserVariable(_engine, "{increment}");
            DateTime.Now.StoreInUserVariable(_engine, "{inputDate}");
            input.StoreInUserVariable(_engine, "{inputDate}");

            _dateCalculation.v_InputDate = "{inputDate}";
            _dateCalculation.v_CalculationMethod = calcMethod;
            _dateCalculation.v_Increment = "{increment}";
            _dateCalculation.v_ToStringFormat = defaultFormat != null ? "{format}":null;
            _dateCalculation.v_OutputUserVariableName = "{output}";

            _dateCalculation.RunCommand(_engine);
            if (expectedResult.GetType() == typeof(DateTime))
            {
                Assert.Equal(expectedResult.ToString(defaultFormat), _dateCalculation.v_OutputUserVariableName.ConvertUserVariableToObject(_engine));
            }
            else if (expectedResult.GetType() == typeof(int))
            {
                Assert.Equal(expectedResult, Int32.Parse(_dateCalculation.v_OutputUserVariableName.ConvertUserVariableToString(_engine)));
            }
        }

        public class DateTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                DateTime inputDate = DateTime.Now;
                string calcMethod = "Add Second(s)";
                int increment = 1;
                DateTime outputDate = inputDate.AddSeconds(increment);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Add Minute(s)";
                outputDate = inputDate.AddMinutes(increment);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Add Hour(s)";
                outputDate = inputDate.AddHours(increment);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Add Day(s)";
                outputDate = inputDate.AddDays(increment);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Add Month(s)";
                outputDate = inputDate.AddMonths(increment);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Add Year(s)";
                outputDate = inputDate.AddYears(increment);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Subtract Second(s)";
                outputDate = inputDate.AddSeconds(increment * -1);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Subtract Minute(s)";
                outputDate = inputDate.AddMinutes(increment * -1);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Subtract Year(s)";
                outputDate = inputDate.AddYears(increment * -1);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Subtract Day(s)";
                outputDate = inputDate.AddDays(increment * -1);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Subtract Month(s)";
                outputDate = inputDate.AddMonths(increment * -1);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Subtract Year(s)";
                outputDate = inputDate.AddYears(increment * -1);
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDate };
                calcMethod = "Get Next Day";
                int outputDateInt = inputDate.AddDays(increment).Day;
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDateInt };
                calcMethod = "Get Next Month";
                outputDateInt = inputDate.AddMonths(increment).Month;
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDateInt };
                calcMethod = "Get Next Year";
                outputDateInt = inputDate.AddYears(increment).Year;
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDateInt };
                calcMethod = "Get Previous Day";
                outputDateInt = inputDate.AddDays(increment * -1).Day;
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDateInt };
                calcMethod = "Get Previous Month";
                outputDateInt = inputDate.AddMonths(increment * -1).Month;
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDateInt };
                calcMethod = "Get Previous Year";
                outputDateInt = inputDate.AddYears(increment * -1).Year;
                yield return new object[] { inputDate, calcMethod, increment.ToString(), outputDateInt };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
