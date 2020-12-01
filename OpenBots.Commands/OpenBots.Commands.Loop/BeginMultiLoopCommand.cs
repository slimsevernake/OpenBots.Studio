using Newtonsoft.Json;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Script;
using OpenBots.Core.UI.Controls;
using OpenBots.Core.UI.Controls.CustomControls;
using OpenBots.Core.Utilities.CommandUtilities;
using OpenBots.Engine;
using OpenBots.UI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.Commands.Loop
{
	[Serializable]
	[Category("Loop Commands")]
	[Description("This command evaluates a group of specified logical statements and executes the contained commands repeatedly (in loop) " +
		"until the result of the logical statements becomes false.")]
	public class BeginMultiLoopCommand : ScriptCommand
	{

		[Required]
		[DisplayName("Multiple Loop Conditions")]
		[Description("Add new Loop condition(s).")]
		[SampleUsage("")]
		[Remarks("All of the conditions must be true to execute the loop block.")]
		[Editor("ShowLoopBuilder", typeof(UIAdditionalHelperType))]
		public DataTable v_LoopConditionsTable { get; set; }

		[JsonIgnore]
		[Browsable(false)]
		private DataGridView _loopConditionHelper;

		[JsonIgnore]
		[Browsable(false)]
		private List<ScriptVariable> _scriptVariables;

		[JsonIgnore]
		[Browsable(false)]
		private List<ScriptElement> _scriptElements;

		public BeginMultiLoopCommand()
		{
			CommandName = "BeginMultiLoopCommand";
			SelectionName = "Begin Multi Loop";
			CommandEnabled = true;            

			v_LoopConditionsTable = new DataTable();
			v_LoopConditionsTable.TableName = DateTime.Now.ToString("MultiLoopConditionTable" + DateTime.Now.ToString("MMddyy.hhmmss"));
			v_LoopConditionsTable.Columns.Add("Statement");
			v_LoopConditionsTable.Columns.Add("CommandData");
		}

		public override void RunCommand(object sender, ScriptAction parentCommand)
		{
			var engine = (AutomationEngineInstance)sender;
			bool isTrueStatement = DetermineMultiStatementTruth(engine);
			engine.ReportProgress("Starting Loop");

			while (isTrueStatement)
			{
				foreach (var cmd in parentCommand.AdditionalScriptCommands)
				{
					if (engine.IsCancellationPending)
						return;

					engine.ExecuteCommand(cmd);

					if (engine.CurrentLoopCancelled)
					{
						engine.ReportProgress("Exiting Loop");
						engine.CurrentLoopCancelled = false;
						return;
					}

					if (engine.CurrentLoopContinuing)
					{
						engine.ReportProgress("Continuing Next Loop");
						engine.CurrentLoopContinuing = false;
						break;
					}
				}
				isTrueStatement = DetermineMultiStatementTruth(engine);
			}
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			//get script variables for feeding into loop builder form
			_scriptVariables = editor.ScriptVariables;
			_scriptElements = editor.ScriptElements;

			//create controls
			var controls = commandControls.CreateDataGridViewGroupFor("v_LoopConditionsTable", this, editor);
			_loopConditionHelper = controls[2] as DataGridView;

			//handle helper click
			var helper = controls[1] as CommandItemControl;
			helper.Click += (sender, e) => CreateLoopCondition(sender, e, editor, commandControls);

			//add for rendering
			RenderedControls.AddRange(controls);

			//define if condition helper
			_loopConditionHelper.Width = 450;
			_loopConditionHelper.Height = 200;
			_loopConditionHelper.AutoGenerateColumns = false;
			_loopConditionHelper.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			_loopConditionHelper.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Condition", DataPropertyName = "Statement", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
			_loopConditionHelper.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CommandData", DataPropertyName = "CommandData", ReadOnly = true, Visible = false });
			_loopConditionHelper.Columns.Add(new DataGridViewButtonColumn() { HeaderText = "Edit", UseColumnTextForButtonValue = true, Text = "Edit", Width = 45 });
			_loopConditionHelper.Columns.Add(new DataGridViewButtonColumn() { HeaderText = "Delete", UseColumnTextForButtonValue = true, Text = "Delete", Width = 60 });
			_loopConditionHelper.AllowUserToAddRows = false;
			_loopConditionHelper.AllowUserToDeleteRows = true;
			_loopConditionHelper.CellContentClick += (sender, e) => LoopConditionHelper_CellContentClick(sender, e, commandControls);

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			if (v_LoopConditionsTable.Rows.Count == 0)
			{
				return "Loop <Not Configured>";
			}
			else
			{
				var statements = v_LoopConditionsTable.AsEnumerable().Select(f => f.Field<string>("Statement")).ToList();
				return string.Join(" && ", statements);
			}
		}

		private bool DetermineMultiStatementTruth(IEngine engine)
		{
			bool isTrueStatement = true;
			foreach (DataRow rw in v_LoopConditionsTable.Rows)
			{
				var commandData = rw["CommandData"].ToString();
				var loopCommand = JsonConvert.DeserializeObject<BeginLoopCommand>(commandData);
				var statementResult = UICommandsHelper.DetermineStatementTruth(engine, loopCommand.v_LoopActionType, loopCommand.v_LoopActionParameterTable);

				if (!statementResult)
				{
					isTrueStatement = false;
					break;
				}
			}
			return isTrueStatement;
		}

		private void LoopConditionHelper_CellContentClick(object sender, DataGridViewCellEventArgs e, ICommandControls commandControls)
		{
			var senderGrid = (DataGridView)sender;

			if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
			{
				var buttonSelected = senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;
				var selectedRow = v_LoopConditionsTable.Rows[e.RowIndex];

				if (buttonSelected.Value.ToString() == "Edit")
				{
					//launch editor
					var statement = selectedRow["Statement"];
					var commandData = selectedRow["CommandData"].ToString();
					var loopCommand = JsonConvert.DeserializeObject<BeginLoopCommand>(commandData);

					var automationCommands = new List<AutomationCommand>() { CommandsHelper.ConvertToAutomationCommand(typeof(BeginLoopCommand)) };
					IfrmCommandEditor editor = commandControls.CreateCommandEditorForm(automationCommands, null);
					editor.SelectedCommand = new BeginLoopCommand(); 
					editor.SelectedCommand = loopCommand;
					editor.EditingCommand = loopCommand;
					editor.OriginalCommand = loopCommand;
					editor.CreationModeInstance = CreationMode.Edit;
					editor.ScriptVariables = _scriptVariables;
					editor.ScriptElements = _scriptElements;

					if (((Form)editor).ShowDialog() == DialogResult.OK)
					{
						var editedCommand = editor.EditingCommand as BeginLoopCommand;
						var displayText = editedCommand.GetDisplayValue();
						var serializedData = JsonConvert.SerializeObject(editedCommand);

						selectedRow["Statement"] = displayText;
						selectedRow["CommandData"] = serializedData;
					}
				}
				else if (buttonSelected.Value.ToString() == "Delete")
				{
					//delete
					v_LoopConditionsTable.Rows.Remove(selectedRow);
				}
				else
				{
					throw new NotImplementedException("Requested Action is not implemented.");
				}
			}
		}

		private void CreateLoopCondition(object sender, EventArgs e, IfrmCommandEditor parentEditor, ICommandControls commandControls)
		{
			var automationCommands = new List<AutomationCommand>() { CommandsHelper.ConvertToAutomationCommand(typeof(BeginLoopCommand)) };
			IfrmCommandEditor editor = commandControls.CreateCommandEditorForm(automationCommands, null);
			editor.SelectedCommand = new BeginLoopCommand();
			editor.ScriptVariables = parentEditor.ScriptVariables;

			if (((Form)editor).ShowDialog() == DialogResult.OK)
			{
				//get data
				var configuredCommand = editor.SelectedCommand as BeginLoopCommand;
				var displayText = configuredCommand.GetDisplayValue();
				var serializedData = JsonConvert.SerializeObject(configuredCommand);
				parentEditor.ScriptVariables = editor.ScriptVariables;

				//add to list
				v_LoopConditionsTable.Rows.Add(displayText, serializedData);
			}
		}
	}
}