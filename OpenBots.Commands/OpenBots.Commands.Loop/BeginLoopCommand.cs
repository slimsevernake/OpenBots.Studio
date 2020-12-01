using Newtonsoft.Json;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Properties;
using OpenBots.Core.Script;
using OpenBots.Core.UI.Controls;
using OpenBots.Engine;
using OpenBots.UI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.Commands.Loop
{
	[Serializable]
	[Category("Loop Commands")]
	[Description("This command evaluates a specified logical statement and executes the contained commands repeatedly (in loop) " +
		"until that logical statement becomes false.")]
	public class BeginLoopCommand : ScriptCommand
	{

		[Required]
		[DisplayName("Loop Condition")]
		[PropertyUISelectionOption("Value Compare")]
		[PropertyUISelectionOption("Date Compare")]
		[PropertyUISelectionOption("Variable Compare")]
		[PropertyUISelectionOption("Variable Has Value")]
		[PropertyUISelectionOption("Variable Is Numeric")]
		[PropertyUISelectionOption("Window Name Exists")]
		[PropertyUISelectionOption("Active Window Name Is")]
		[PropertyUISelectionOption("File Exists")]
		[PropertyUISelectionOption("Folder Exists")]
		[PropertyUISelectionOption("Web Element Exists")]
		[PropertyUISelectionOption("GUI Element Exists")]
		[PropertyUISelectionOption("Image Element Exists")]
		[PropertyUISelectionOption("App Instance Exists")]
		[PropertyUISelectionOption("Error Occured")]
		[PropertyUISelectionOption("Error Did Not Occur")]
		[Description("Select the necessary condition type.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_LoopActionType { get; set; }

		[Required]
		[DisplayName("Additional Parameters")]
		[Description("Supply or Select the required comparison parameters.")]
		[SampleUsage("Param Value || {vParamValue}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public DataTable v_LoopActionParameterTable { get; set; }

		[JsonIgnore]
		[Browsable(false)]
		private DataGridView _loopGridViewHelper;

		[JsonIgnore]
		[Browsable(false)]
		private ComboBox _actionDropdown;

		[JsonIgnore]
		[Browsable(false)]
		private List<Control> _parameterControls;

		[JsonIgnore]
		[Browsable(false)]
		private CommandItemControl _recorderControl;

		public BeginLoopCommand()
		{
			CommandName = "BeginLoopCommand";
			SelectionName = "Begin Loop";
			CommandEnabled = true;         

			//define parameter table
			v_LoopActionParameterTable = new DataTable
			{
				TableName = DateTime.Now.ToString("LoopActionParamTable" + DateTime.Now.ToString("MMddyy.hhmmss"))
			};
			v_LoopActionParameterTable.Columns.Add("Parameter Name");
			v_LoopActionParameterTable.Columns.Add("Parameter Value");

			_loopGridViewHelper = new DataGridView();
			_loopGridViewHelper.AllowUserToAddRows = true;
			_loopGridViewHelper.AllowUserToDeleteRows = true;
			_loopGridViewHelper.Size = new Size(400, 250);
			_loopGridViewHelper.ColumnHeadersHeight = 30;
			_loopGridViewHelper.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			_loopGridViewHelper.DataBindings.Add("DataSource", this, "v_LoopActionParameterTable", false, DataSourceUpdateMode.OnPropertyChanged);
			_loopGridViewHelper.AllowUserToAddRows = false;
			_loopGridViewHelper.AllowUserToDeleteRows = false;
			_loopGridViewHelper.MouseEnter += LoopGridViewHelper_MouseEnter;

			_recorderControl = new CommandItemControl();
			_recorderControl.Padding = new Padding(10, 0, 0, 0);
			_recorderControl.ForeColor = Color.AliceBlue;
			_recorderControl.Font = new Font("Segoe UI Semilight", 10);
			_recorderControl.Name = "guirecorder_helper";
			_recorderControl.CommandImage = Resources.command_camera;
			_recorderControl.CommandDisplay = "Element Recorder";
			_recorderControl.Hide();
		}

		public override void RunCommand(object sender, ScriptAction parentCommand)
		{
			var engine = (AutomationEngineInstance)sender;
			var loopResult = UICommandsHelper.DetermineStatementTruth(engine, v_LoopActionType, v_LoopActionParameterTable);
			engine.ReportProgress("Starting Loop"); 

			while (loopResult)
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
				loopResult = UICommandsHelper.DetermineStatementTruth(engine, v_LoopActionType, v_LoopActionParameterTable);
			}
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			_actionDropdown = (ComboBox)commandControls.CreateDropdownFor("v_LoopActionType", this);
			RenderedControls.Add(commandControls.CreateDefaultLabelFor("v_LoopActionType", this));
			RenderedControls.AddRange(commandControls.CreateUIHelpersFor("v_LoopActionType", this, new Control[] { _actionDropdown }, editor));
			_actionDropdown.SelectionChangeCommitted += loopAction_SelectionChangeCommitted;
			RenderedControls.Add(_actionDropdown);

			_parameterControls = new List<Control>();
			_parameterControls.Add(commandControls.CreateDefaultLabelFor("v_LoopActionParameterTable", this));
			_recorderControl.Click += (sender, e) => ShowLoopElementRecorder(sender, e, editor, commandControls);
			_parameterControls.Add(_recorderControl);
			_parameterControls.AddRange(commandControls.CreateUIHelpersFor("v_LoopActionParameterTable", this, new Control[] { _loopGridViewHelper }, editor));
			_parameterControls.Add(_loopGridViewHelper);
			RenderedControls.AddRange(_parameterControls);

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			switch (v_LoopActionType)
			{
				case "Value Compare":
				case "Date Compare":
				case "Variable Compare":
					string value1 = ((from rw in v_LoopActionParameterTable.AsEnumerable()
									  where rw.Field<string>("Parameter Name") == "Value1"
									  select rw.Field<string>("Parameter Value")).FirstOrDefault());
					string operand = ((from rw in v_LoopActionParameterTable.AsEnumerable()
									   where rw.Field<string>("Parameter Name") == "Operand"
									   select rw.Field<string>("Parameter Value")).FirstOrDefault());
					string value2 = ((from rw in v_LoopActionParameterTable.AsEnumerable()
									  where rw.Field<string>("Parameter Name") == "Value2"
									  select rw.Field<string>("Parameter Value")).FirstOrDefault());

					return $"Loop While ('{value1}' {operand} '{value2}')";

				case "Variable Has Value":
					string variableName = ((from rw in v_LoopActionParameterTable.AsEnumerable()
											where rw.Field<string>("Parameter Name") == "Variable Name"
											select rw.Field<string>("Parameter Value")).FirstOrDefault());

					return $"Loop While (Variable '{variableName}' Has Value)";

				case "Variable Is Numeric":
					string varName = ((from rw in v_LoopActionParameterTable.AsEnumerable()
									   where rw.Field<string>("Parameter Name") == "Variable Name"
									   select rw.Field<string>("Parameter Value")).FirstOrDefault());

					return $"Loop While (Variable '{varName}' Is Numeric)";

				case "Error Occured":
					string lineNumber = ((from rw in v_LoopActionParameterTable.AsEnumerable()
										  where rw.Field<string>("Parameter Name") == "Line Number"
										  select rw.Field<string>("Parameter Value")).FirstOrDefault());

					return $"Loop While (Error Occured on Line Number '{lineNumber}')";

				case "Error Did Not Occur":
					string lineNum = ((from rw in v_LoopActionParameterTable.AsEnumerable()
									   where rw.Field<string>("Parameter Name") == "Line Number"
									   select rw.Field<string>("Parameter Value")).FirstOrDefault());

					return $"Loop While (Error Did Not Occur on Line Number '{lineNum}')";

				case "Window Name Exists":
				case "Active Window Name Is":

					string windowName = ((from rw in v_LoopActionParameterTable.AsEnumerable()
										  where rw.Field<string>("Parameter Name") == "Window Name"
										  select rw.Field<string>("Parameter Value")).FirstOrDefault());

					return $"Loop While {v_LoopActionType} [Window Name '{windowName}']";

				case "File Exists":
					string filePath = ((from rw in v_LoopActionParameterTable.AsEnumerable()
										where rw.Field<string>("Parameter Name") == "File Path"
										select rw.Field<string>("Parameter Value")).FirstOrDefault());

					string fileCompareType = ((from rw in v_LoopActionParameterTable.AsEnumerable()
											   where rw.Field<string>("Parameter Name") == "True When"
											   select rw.Field<string>("Parameter Value")).FirstOrDefault());

					if (fileCompareType == "It Does Not Exist")
						return $"Loop While File Does Not Exist [File '{filePath}']";
					else
						return $"Loop While File Exists [File '{filePath}']";

				case "Folder Exists":
					string folderPath = ((from rw in v_LoopActionParameterTable.AsEnumerable()
										  where rw.Field<string>("Parameter Name") == "Folder Path"
										  select rw.Field<string>("Parameter Value")).FirstOrDefault());

					string folderCompareType = ((from rw in v_LoopActionParameterTable.AsEnumerable()
												 where rw.Field<string>("Parameter Name") == "True When"
												 select rw.Field<string>("Parameter Value")).FirstOrDefault());

					if (folderCompareType == "It Does Not Exist")
						return $"Loop While Folder Does Not Exist [Folder '{folderPath}']";
					else
						return $"Loop While Folder Exists [Folder '{folderPath}']";

				case "Web Element Exists":
					string parameterName = ((from rw in v_LoopActionParameterTable.AsEnumerable()
											 where rw.Field<string>("Parameter Name") == "Element Search Parameter"
											 select rw.Field<string>("Parameter Value")).FirstOrDefault());

					string searchMethod = ((from rw in v_LoopActionParameterTable.AsEnumerable()
											where rw.Field<string>("Parameter Name") == "Element Search Method"
											select rw.Field<string>("Parameter Value")).FirstOrDefault());

					string webElementCompareType = ((from rw in v_LoopActionParameterTable.AsEnumerable()
													 where rw.Field<string>("Parameter Name") == "True When"
													 select rw.Field<string>("Parameter Value")).FirstOrDefault());

					if (webElementCompareType == "It Does Not Exist")
						return $"Loop While Web Element Does Not Exist [{searchMethod} '{parameterName}']";
					else
						return $"Loop While Web Element Exists [{searchMethod} '{parameterName}']";

				case "GUI Element Exists":
					string guiWindowName = ((from rw in v_LoopActionParameterTable.AsEnumerable()
											 where rw.Field<string>("Parameter Name") == "Window Name"
											 select rw.Field<string>("Parameter Value")).FirstOrDefault());

					string guiSearch = ((from rw in v_LoopActionParameterTable.AsEnumerable()
										 where rw.Field<string>("Parameter Name") == "Element Search Parameter"
										 select rw.Field<string>("Parameter Value")).FirstOrDefault());

					string guiElementCompareType = ((from rw in v_LoopActionParameterTable.AsEnumerable()
													 where rw.Field<string>("Parameter Name") == "True When"
													 select rw.Field<string>("Parameter Value")).FirstOrDefault());

					if (guiElementCompareType == "It Does Not Exist")
						return $"Loop While GUI Element Does Not Exist [Find '{guiSearch}' Element In '{guiWindowName}']";
					else
						return $"Loop While GUI Element Exists [Find '{guiSearch}' Element In '{guiWindowName}']";

				case "Image Element Exists":
					string imageCompareType = (from rw in v_LoopActionParameterTable.AsEnumerable()
											   where rw.Field<string>("Parameter Name") == "True When"
											   select rw.Field<string>("Parameter Value")).FirstOrDefault();

					if (imageCompareType == "It Does Not Exist")
						return $"Loop While Image Does Not Exist on Screen";
					else
						return $"Loop While Image Exists on Screen";
				case "App Instance Exists":
					string instanceName = ((from rw in v_LoopActionParameterTable.AsEnumerable()
											where rw.Field<string>("Parameter Name") == "Instance Name"
											select rw.Field<string>("Parameter Value")).FirstOrDefault());

					string instanceCompareType = (from rw in v_LoopActionParameterTable.AsEnumerable()
												  where rw.Field<string>("Parameter Name") == "True When"
												  select rw.Field<string>("Parameter Value")).FirstOrDefault();

					if (instanceCompareType == "It Does Not Exist")
						return $"Loop While App Instance Does Not Exist [Instance Name '{instanceName}']";
					else
						return $"Loop While App Instance Exists [Instance Name '{instanceName}']";
				default:
					return "Loop While ...";
			}

		}
		
		private void loopAction_SelectionChangeCommitted(object sender, EventArgs e)
		{
			DataGridView loopActionParameterBox = _loopGridViewHelper;

			BeginLoopCommand cmd = this;
			DataTable actionParameters = cmd.v_LoopActionParameterTable;

			//sender is null when command is updating
			if (sender != null)
				actionParameters.Rows.Clear();

			DataGridViewComboBoxCell comparisonComboBox;

			//remove if exists            
			if (_recorderControl.Visible)
			{
				_recorderControl.Hide();
			}

			switch (_actionDropdown.SelectedItem)
			{
				case "Value Compare":
				case "Date Compare":

					loopActionParameterBox.Visible = true;

					if (sender != null)
					{
						actionParameters.Rows.Add("Value1", "");
						actionParameters.Rows.Add("Operand", "");
						actionParameters.Rows.Add("Value2", "");
						loopActionParameterBox.DataSource = actionParameters;
					}

					//combobox cell for Variable Name
					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("is equal to");
					comparisonComboBox.Items.Add("is greater than");
					comparisonComboBox.Items.Add("is greater than or equal to");
					comparisonComboBox.Items.Add("is less than");
					comparisonComboBox.Items.Add("is less than or equal to");
					comparisonComboBox.Items.Add("is not equal to");

					//assign cell as a combobox
					loopActionParameterBox.Rows[1].Cells[1] = comparisonComboBox;

					break;
				case "Variable Compare":

					loopActionParameterBox.Visible = true;

					if (sender != null)
					{
						actionParameters.Rows.Add("Value1", "");
						actionParameters.Rows.Add("Operand", "");
						actionParameters.Rows.Add("Value2", "");
						actionParameters.Rows.Add("Case Sensitive", "No");
						loopActionParameterBox.DataSource = actionParameters;
					}

					//combobox cell for Variable Name
					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("contains");
					comparisonComboBox.Items.Add("does not contain");
					comparisonComboBox.Items.Add("is equal to");
					comparisonComboBox.Items.Add("is not equal to");

					//assign cell as a combobox
					loopActionParameterBox.Rows[1].Cells[1] = comparisonComboBox;

					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("Yes");
					comparisonComboBox.Items.Add("No");

					//assign cell as a combobox
					loopActionParameterBox.Rows[3].Cells[1] = comparisonComboBox;

					break;
				case "Variable Has Value":

					loopActionParameterBox.Visible = true;
					if (sender != null)
					{
						actionParameters.Rows.Add("Variable Name", "");
						loopActionParameterBox.DataSource = actionParameters;
					}

					break;
				case "Variable Is Numeric":

					loopActionParameterBox.Visible = true;
					if (sender != null)
					{
						actionParameters.Rows.Add("Variable Name", "");
						loopActionParameterBox.DataSource = actionParameters;
					}

					break;
				case "Error Occured":

					loopActionParameterBox.Visible = true;
					if (sender != null)
					{
						actionParameters.Rows.Add("Line Number", "");
						loopActionParameterBox.DataSource = actionParameters;
					}

					break;
				case "Error Did Not Occur":

					loopActionParameterBox.Visible = true;

					if (sender != null)
					{
						actionParameters.Rows.Add("Line Number", "");
						loopActionParameterBox.DataSource = actionParameters;
					}

					break;
				case "Window Name Exists":
				case "Active Window Name Is":

					loopActionParameterBox.Visible = true;
					if (sender != null)
					{
						actionParameters.Rows.Add("Window Name", "");
						loopActionParameterBox.DataSource = actionParameters;
					}

					break;
				case "File Exists":

					loopActionParameterBox.Visible = true;
					if (sender != null)
					{
						actionParameters.Rows.Add("File Path", "");
						actionParameters.Rows.Add("True When", "It Does Exist");
						loopActionParameterBox.DataSource = actionParameters;
					}


					//combobox cell for Variable Name
					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("It Does Exist");
					comparisonComboBox.Items.Add("It Does Not Exist");

					//assign cell as a combobox
					loopActionParameterBox.Rows[1].Cells[1] = comparisonComboBox;

					break;
				case "Folder Exists":

					loopActionParameterBox.Visible = true;


					if (sender != null)
					{
						actionParameters.Rows.Add("Folder Path", "");
						actionParameters.Rows.Add("True When", "It Does Exist");
						loopActionParameterBox.DataSource = actionParameters;
					}

					//combobox cell for Variable Name
					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("It Does Exist");
					comparisonComboBox.Items.Add("It Does Not Exist");

					//assign cell as a combobox
					loopActionParameterBox.Rows[1].Cells[1] = comparisonComboBox;
					break;
				case "Web Element Exists":

					loopActionParameterBox.Visible = true;

					if (sender != null)
					{
						actionParameters.Rows.Add("Selenium Instance Name", "DefaultBrowser");
						actionParameters.Rows.Add("Element Search Method", "");
						actionParameters.Rows.Add("Element Search Parameter", "");
						actionParameters.Rows.Add("Timeout (Seconds)", "30");
						actionParameters.Rows.Add("True When", "It Does Exist");
						loopActionParameterBox.DataSource = actionParameters;
					}

					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("It Does Exist");
					comparisonComboBox.Items.Add("It Does Not Exist");

					//assign cell as a combobox
					loopActionParameterBox.Rows[4].Cells[1] = comparisonComboBox;

					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("XPath");
					comparisonComboBox.Items.Add("ID");
					comparisonComboBox.Items.Add("Name");
					comparisonComboBox.Items.Add("Tag Name");
					comparisonComboBox.Items.Add("Class Name");
					comparisonComboBox.Items.Add("CSS Selector");

					//assign cell as a combobox
					loopActionParameterBox.Rows[1].Cells[1] = comparisonComboBox;

					break;
				case "GUI Element Exists":

					loopActionParameterBox.Visible = true;
					if (sender != null)
					{
						actionParameters.Rows.Add("Window Name", "Current Window");
						actionParameters.Rows.Add("Element Search Method", "");
						actionParameters.Rows.Add("Element Search Parameter", "");
						actionParameters.Rows.Add("True When", "It Does Exist");
						loopActionParameterBox.DataSource = actionParameters;
					}

					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("It Does Exist");
					comparisonComboBox.Items.Add("It Does Not Exist");

					//assign cell as a combobox
					loopActionParameterBox.Rows[3].Cells[1] = comparisonComboBox;

					var parameterName = new DataGridViewComboBoxCell();
					parameterName.Items.Add("AcceleratorKey");
					parameterName.Items.Add("AccessKey");
					parameterName.Items.Add("AutomationId");
					parameterName.Items.Add("ClassName");
					parameterName.Items.Add("FrameworkId");
					parameterName.Items.Add("HasKeyboardFocus");
					parameterName.Items.Add("HelpText");
					parameterName.Items.Add("IsContentElement");
					parameterName.Items.Add("IsControlElement");
					parameterName.Items.Add("IsEnabled");
					parameterName.Items.Add("IsKeyboardFocusable");
					parameterName.Items.Add("IsOffscreen");
					parameterName.Items.Add("IsPassword");
					parameterName.Items.Add("IsRequiredForForm");
					parameterName.Items.Add("ItemStatus");
					parameterName.Items.Add("ItemType");
					parameterName.Items.Add("LocalizedControlType");
					parameterName.Items.Add("Name");
					parameterName.Items.Add("NativeWindowHandle");
					parameterName.Items.Add("ProcessID");

					//assign cell as a combobox
					loopActionParameterBox.Rows[1].Cells[1] = parameterName;

					_recorderControl.Show();
					break;
				case "Image Element Exists":
					loopActionParameterBox.Visible = true;

					if (sender != null)
					{
						actionParameters.Rows.Add("Captured Image Variable", "");
						actionParameters.Rows.Add("Accuracy (0-1)", "0.8");
						actionParameters.Rows.Add("True When", "It Does Exist");
						loopActionParameterBox.DataSource = actionParameters;
					}

					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("It Does Exist");
					comparisonComboBox.Items.Add("It Does Not Exist");

					//assign cell as a combobox
					loopActionParameterBox.Rows[2].Cells[1] = comparisonComboBox;
					break;
				case "App Instance Exists":
					loopActionParameterBox.Visible = true;

					if (sender != null)
					{
						actionParameters.Rows.Add("Instance Name", "");
						actionParameters.Rows.Add("True When", "It Does Exist");
						loopActionParameterBox.DataSource = actionParameters;
					}

					comparisonComboBox = new DataGridViewComboBoxCell();
					comparisonComboBox.Items.Add("It Does Exist");
					comparisonComboBox.Items.Add("It Does Not Exist");

					//assign cell as a combobox
					loopActionParameterBox.Rows[1].Cells[1] = comparisonComboBox;
					break;
				default:
					break;
			}
		}

		private void LoopGridViewHelper_MouseEnter(object sender, EventArgs e)
		{
			try
			{
				loopAction_SelectionChangeCommitted(null, null);
			}
			catch (Exception)
			{
				loopAction_SelectionChangeCommitted(sender, e);
			}
		}

		private void ShowLoopElementRecorder(object sender, EventArgs e, IfrmCommandEditor editor, ICommandControls commandControls)
		{
			_loopGridViewHelper.Rows[0].Cells[1].Value = commandControls.ShowCommandsElementRecorder(sender, e, editor);
		}
	}
}
