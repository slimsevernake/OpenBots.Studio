using Newtonsoft.Json;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.Commands.Data
{
	[Serializable]
	[Category("Data Commands")]
	[Description("This command creates a DateTime and saves the result in a variable.")]
	public class CreateDateTimeCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Year")]
		[Description("Select or provide the year.")]
		[SampleUsage("2020 || {vYear}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Year { get; set; }

		[Required]
		[DisplayName("Month")]
		[Description("Select or provide the month.")]
		[SampleUsage("3 || 03 || january || jan || {vMonth}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Month { get; set; }

		[Required]
		[DisplayName("Day")]
		[Description("Select or provide the day.")]
		[SampleUsage("20 || {vDay}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Day { get; set; }

		[DisplayName("Time (Optional)")]
		[Description("Select or provide the hour.")]
		[SampleUsage("5:15 || 8:30:10 || {vTime}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Time { get; set; }

		[DisplayName("Period (Optional)")]
		[PropertyUISelectionOption("AM")]
		[PropertyUISelectionOption("PM")]
		[Description("Select whether the time is AM or PM")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_AMorPM { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output Date Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public CreateDateTimeCommand()
		{
			CommandName = "CreateDateTimeCommand";
			SelectionName = "Create DateTime";
			CommandEnabled = true;
			v_AMorPM = "AM";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			var vYear = int.Parse(v_Year.ConvertUserVariableToString(engine));
			var vMonth = v_Month.ConvertUserVariableToString(engine);
			var vDay = int.Parse(v_Day.ConvertUserVariableToString(engine));
			var vTime = v_Time.ConvertUserVariableToString(engine);


			if (!int.TryParse(vMonth, out int vMonthInt))
			{
				Dictionary<string, int> monthDict = new Dictionary<string, int>()
				{
					{ "jan", 1 },
					{ "feb", 2 },
					{ "mar", 3 },
					{ "apr", 4 },
					{ "may", 5 },
					{ "jun", 6 },
					{ "jul", 7 },
					{ "aug", 8 },
					{ "sep", 9 },
					{ "oct", 10 },
					{ "nov", 11 },
					{ "dec", 12 },
				};
				vMonthInt = monthDict[vMonth.ToLower().Substring(0,3)];
			}
			

			DateTime date;
			if (!string.IsNullOrEmpty(vTime))
			{
				var vHour = int.Parse(vTime.Split(':')[0]);

				if (v_AMorPM == "PM")
					vHour += 12;

				var vMinute = int.Parse(vTime.Split(':')[1]);

				int vSecond = 0;
				if (vTime.Split(':').Length == 3)
					vSecond = int.Parse(vTime.Split(':')[2]);


				date = new DateTime(vYear, vMonthInt, vDay, vHour, vMinute, vSecond);

			}
			else
				date = new DateTime(vYear, vMonthInt, vDay);

			date.StoreInUserVariable(engine, v_OutputUserVariableName);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Year", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Month", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Day", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Time", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_AMorPM", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
		
			return base.GetDisplayValue() + $" [Year '{v_Year}' - Month '{v_Month}'- Day '{v_Day}' Time '{v_Time}{v_AMorPM}'- Store Date in '{v_OutputUserVariableName}']";
		}

		
	}
}