using Newtonsoft.Json;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Common;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.UI.Controls;
using OpenBots.Core.User32;
using OpenBots.Core.Utilities.CommandUtilities;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Core.Utilities.FormsUtilities;
using OpenBots.Engine;
using OpenBots.UI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;

namespace OpenBots.Commands.Image
{
	[Serializable]
	[Category("Image Commands")]
	[Description("This command attempts to find and perform an action on an existing image on screen.")]
	public class SurfaceAutomationCommand : ScriptCommand, IImageCommands
	{
		[Required]
		[DisplayName("Capture Search Image")]
		[Description("Use the tool to capture an image that will be located on screen during execution.")]
		[SampleUsage("")]
		[Remarks("Images with larger color variance will be found more quickly than those with a lot of white space. \n" +
				 "For images that are primarily white space, tagging color to the top-left corner of the image and setting \n" +
				 "the relative click position will produce faster results.")]
		[Editor("ShowImageCaptureHelper", typeof(UIAdditionalHelperType))]
		public string v_ImageCapture { get; set; }

		[Required]
		[DisplayName("Element Action")]
		[PropertyUISelectionOption("Click Image")]
		[PropertyUISelectionOption("Set Text")]
		[PropertyUISelectionOption("Set Secure Text")]
		[PropertyUISelectionOption("Check If Image Exists")]
		[PropertyUISelectionOption("Wait For Image To Exist")]
		[Description("Select the appropriate corresponding action to take once the image has been located.")]
		[SampleUsage("")]
		[Remarks("Selecting this field changes the parameters required in the following step.")]
		public string v_ImageAction { get; set; }

		[Required]
		[DisplayName("Additional Parameters")]
		[Description("Additional Parameters will be required based on the action settings selected.")]
		[SampleUsage("data || {vData}")]
		[Remarks("Additional Parameters range from adding offset coordinates to specifying a variable to apply element text to.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public DataTable v_ImageActionParameterTable { get; set; }

		[Required]
		[DisplayName("Accuracy (0-1)")]
		[Description("Enter a value between 0 and 1 to set the match Accuracy. Set to 1 for a perfect match.")]
		[SampleUsage("0.8 || 1 || {vAccuracy}")]
		[Remarks("Accuracy must be a value between 0 and 1.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_MatchAccuracy { get; set; }

		[JsonIgnore]
		[Browsable(false)]
		public bool TestMode { get; set; } = false;

		[JsonIgnore]
		[Browsable(false)]
		private DataGridView _imageGridViewHelper;

		[JsonIgnore]
		[Browsable(false)]
		private ComboBox _imageActionDropdown;

		[JsonIgnore]
		[Browsable(false)]
		private List<Control> _imageParameterControls;

		public SurfaceAutomationCommand()
		{
			CommandName = "SurfaceAutomationCommand";
			SelectionName = "Surface Automation";
			CommandEnabled = true;

			v_MatchAccuracy = "0.8";

			v_ImageActionParameterTable = new DataTable
			{
				TableName = "ImageActionParamTable" + DateTime.Now.ToString("MMddyy.hhmmss")
			};
			v_ImageActionParameterTable.Columns.Add("Parameter Name");
			v_ImageActionParameterTable.Columns.Add("Parameter Value");

			_imageGridViewHelper = new DataGridView();
			_imageGridViewHelper.AllowUserToAddRows = true;
			_imageGridViewHelper.AllowUserToDeleteRows = true;
			_imageGridViewHelper.Size = new Size(400, 250);
			_imageGridViewHelper.ColumnHeadersHeight = 30;
			_imageGridViewHelper.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			_imageGridViewHelper.DataBindings.Add("DataSource", this, "v_ImageActionParameterTable", false, DataSourceUpdateMode.OnPropertyChanged);
			_imageGridViewHelper.AllowUserToAddRows = false;
			_imageGridViewHelper.AllowUserToDeleteRows = false;
			//_imageGridViewHelper.AllowUserToResizeRows = false;
			_imageGridViewHelper.MouseEnter += ImageGridViewHelper_MouseEnter;
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			bool testMode = TestMode;
			//user image to bitmap
			Bitmap userImage = new Bitmap(Common.Base64ToImage(v_ImageCapture));
			double accuracy;
			try
			{
				accuracy = double.Parse(v_MatchAccuracy.ConvertUserVariableToString(engine));
				if (accuracy > 1 || accuracy < 0)
					throw new ArgumentOutOfRangeException("Accuracy value is out of range (0-1)");
			}
			catch (Exception)
			{
				throw new InvalidDataException("Accuracy value is invalid");
			}

			if (testMode)
			{
				UICommandsHelper.FindImageElement(userImage, accuracy, TestMode);
				return;
			}

			dynamic element = null;
			if (v_ImageAction == "Wait For Image To Exist")
			{
				var timeoutText = (from rw in v_ImageActionParameterTable.AsEnumerable()
								   where rw.Field<string>("Parameter Name") == "Timeout (Seconds)"
								   select rw.Field<string>("Parameter Value")).FirstOrDefault();

				timeoutText = timeoutText.ConvertUserVariableToString(engine);
				int timeOut = Convert.ToInt32(timeoutText);
				var timeToEnd = DateTime.Now.AddSeconds(timeOut);

				while (timeToEnd >= DateTime.Now)
				{
					try
					{
						element = UICommandsHelper.FindImageElement(userImage, accuracy, TestMode);

						if (element == null)
							throw new Exception("Image Element Not Found");
						else
							break;
					}
					catch (Exception)
					{
						engine.ReportProgress("Element Not Yet Found... " + (timeToEnd - DateTime.Now).Seconds + "s remain");
						Thread.Sleep(1000);
					}
				}

				if (element == null)
					throw new Exception("Image Element Not Found");

				return;
			}
			else
				element = UICommandsHelper.FindImageElement(userImage, accuracy, TestMode);

			try
			{
				string clickPosition;
				int xAdjust;
				int yAdjust;
				switch (v_ImageAction)
				{
					case "Click Image":
						string clickType = (from rw in v_ImageActionParameterTable.AsEnumerable()
											where rw.Field<string>("Parameter Name") == "Click Type"
											select rw.Field<string>("Parameter Value")).FirstOrDefault();
						clickPosition = (from rw in v_ImageActionParameterTable.AsEnumerable()
										 where rw.Field<string>("Parameter Name") == "Click Position"
										 select rw.Field<string>("Parameter Value")).FirstOrDefault();
						xAdjust = Convert.ToInt32((from rw in v_ImageActionParameterTable.AsEnumerable()
												   where rw.Field<string>("Parameter Name") == "X Adjustment"
												   select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));
						yAdjust = Convert.ToInt32((from rw in v_ImageActionParameterTable.AsEnumerable()
												   where rw.Field<string>("Parameter Name") == "Y Adjustment"
												   select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));

						Point clickPositionPoint = GetClickPosition(clickPosition, element);

						//move mouse to position
						var mouseX = (clickPositionPoint.X + xAdjust).ToString();
						var mouseY = (clickPositionPoint.Y + yAdjust).ToString();
						User32Functions.SendMouseMove(mouseX, mouseY, clickType);
						break;

					case "Set Text":
						string textToSet = (from rw in v_ImageActionParameterTable.AsEnumerable()
											where rw.Field<string>("Parameter Name") == "Text To Set"
											select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine);
						clickPosition = (from rw in v_ImageActionParameterTable.AsEnumerable()
										 where rw.Field<string>("Parameter Name") == "Click Position"
										 select rw.Field<string>("Parameter Value")).FirstOrDefault();
						xAdjust = Convert.ToInt32((from rw in v_ImageActionParameterTable.AsEnumerable()
												   where rw.Field<string>("Parameter Name") == "X Adjustment"
												   select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));
						yAdjust = Convert.ToInt32((from rw in v_ImageActionParameterTable.AsEnumerable()
												   where rw.Field<string>("Parameter Name") == "Y Adjustment"
												   select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));
						string encryptedData = (from rw in v_ImageActionParameterTable.AsEnumerable()
												where rw.Field<string>("Parameter Name") == "Encrypted Text"
												select rw.Field<string>("Parameter Value")).FirstOrDefault();

						if (encryptedData == "Encrypted")
							textToSet = EncryptionServices.DecryptString(textToSet, "OPENBOTS");

						Point setTextPositionPoint = GetClickPosition(clickPosition, element);

						//move mouse to position and set text
						var xPos = (setTextPositionPoint.X + xAdjust).ToString();
						var yPos = (setTextPositionPoint.Y + yAdjust).ToString();
						User32Functions.SendMouseMove(xPos, yPos, "Left Click");

						var simulator = new InputSimulator();
						simulator.Keyboard.TextEntry(textToSet);
						Thread.Sleep(100);
						break;

					case "Set Secure Text":
						var secureString = (from rw in v_ImageActionParameterTable.AsEnumerable()
											where rw.Field<string>("Parameter Name") == "Secure String Variable"
											select rw.Field<string>("Parameter Value")).FirstOrDefault();
						clickPosition = (from rw in v_ImageActionParameterTable.AsEnumerable()
										 where rw.Field<string>("Parameter Name") == "Click Position"
										 select rw.Field<string>("Parameter Value")).FirstOrDefault();
						xAdjust = Convert.ToInt32((from rw in v_ImageActionParameterTable.AsEnumerable()
												   where rw.Field<string>("Parameter Name") == "X Adjustment"
												   select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));
						yAdjust = Convert.ToInt32((from rw in v_ImageActionParameterTable.AsEnumerable()
												   where rw.Field<string>("Parameter Name") == "Y Adjustment"
												   select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));

						var secureStrVariable = secureString.ConvertUserVariableToObject(engine);

						if (secureStrVariable is SecureString)
							secureString = ((SecureString)secureStrVariable).ConvertSecureStringToString();
						else
							throw new ArgumentException("Provided Argument is not a 'Secure String'");

						Point setSecureTextPositionPoint = GetClickPosition(clickPosition, element);

						//move mouse to position and set text
						var xPosition = (setSecureTextPositionPoint.X + xAdjust).ToString();
						var yPosition = (setSecureTextPositionPoint.Y + yAdjust).ToString();
						User32Functions.SendMouseMove(xPosition, yPosition, "Left Click");

						var simulator2 = new InputSimulator();
						simulator2.Keyboard.TextEntry(secureString);
						Thread.Sleep(100);
						break;

					case "Check If Image Exists":
						var outputVariable = (from rw in v_ImageActionParameterTable.AsEnumerable()
											  where rw.Field<string>("Parameter Name") == "Output Bool Variable Name"
											  select rw.Field<string>("Parameter Value")).FirstOrDefault();

						//remove brackets from variable
						outputVariable = outputVariable.Replace("{", "").Replace("}", "");

						if (element != null)
							"True".StoreInUserVariable(engine, outputVariable);
						else
							"False".StoreInUserVariable(engine, outputVariable);
						break;
					default:
						break;
				}
				FormsHelper.ShowAllForms();
			}
			catch (Exception ex)
			{
				FormsHelper.ShowAllForms();
				if (element == null)
					throw new Exception("Specified image was not found in window!");
				else
					throw ex;
			}
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			UIPictureBox imageCapture = new UIPictureBox();
			imageCapture.Width = 200;
			imageCapture.Height = 200;
			imageCapture.DataBindings.Add("EncodedImage", this, "v_ImageCapture", false, DataSourceUpdateMode.OnPropertyChanged);

			RenderedControls.Add(commandControls.CreateDefaultLabelFor("v_ImageCapture", this));
			RenderedControls.AddRange(commandControls.CreateUIHelpersFor("v_ImageCapture", this, new Control[] { imageCapture }, editor));
			RenderedControls.Add(imageCapture);

			_imageActionDropdown = (ComboBox)commandControls.CreateDropdownFor("v_ImageAction", this);
			RenderedControls.Add(commandControls.CreateDefaultLabelFor("v_ImageAction", this));
			RenderedControls.AddRange(commandControls.CreateUIHelpersFor("v_ImageAction", this, new Control[] { _imageActionDropdown }, editor));
			_imageActionDropdown.SelectionChangeCommitted += ImageAction_SelectionChangeCommitted;
			RenderedControls.Add(_imageActionDropdown);

			_imageParameterControls = new List<Control>();
			_imageParameterControls.Add(commandControls.CreateDefaultLabelFor("v_ImageActionParameterTable", this));
			_imageParameterControls.AddRange(commandControls.CreateUIHelpersFor("v_ImageActionParameterTable", this, new Control[] { _imageGridViewHelper }, editor));
			_imageParameterControls.Add(_imageGridViewHelper);
			RenderedControls.AddRange(_imageParameterControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_MatchAccuracy", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [{v_ImageAction} on Screen - Accuracy '{v_MatchAccuracy}']";
		}
		
		private void ImageAction_SelectionChangeCommitted(object sender, EventArgs e)
		{
			SurfaceAutomationCommand cmd = this;
			DataTable actionParameters = cmd.v_ImageActionParameterTable;

			if (sender != null)
				actionParameters.Rows.Clear();

			DataGridViewComboBoxCell mouseClickPositionBox = new DataGridViewComboBoxCell();
			mouseClickPositionBox.Items.Add("Center");
			mouseClickPositionBox.Items.Add("Top Left");
			mouseClickPositionBox.Items.Add("Top Middle");
			mouseClickPositionBox.Items.Add("Top Right");
			mouseClickPositionBox.Items.Add("Bottom Left");
			mouseClickPositionBox.Items.Add("Bottom Middle");
			mouseClickPositionBox.Items.Add("Bottom Right");
			mouseClickPositionBox.Items.Add("Middle Left");
			mouseClickPositionBox.Items.Add("Middle Right");

			switch (_imageActionDropdown.SelectedItem)
			{
				case "Click Image":
					foreach (var ctrl in _imageParameterControls)
						ctrl.Show();

					DataGridViewComboBoxCell mouseClickTypeBox = new DataGridViewComboBoxCell();
					mouseClickTypeBox.Items.Add("Left Click");
					mouseClickTypeBox.Items.Add("Middle Click");
					mouseClickTypeBox.Items.Add("Right Click");
					mouseClickTypeBox.Items.Add("Left Down");
					mouseClickTypeBox.Items.Add("Middle Down");
					mouseClickTypeBox.Items.Add("Right Down");
					mouseClickTypeBox.Items.Add("Left Up");
					mouseClickTypeBox.Items.Add("Middle Up");
					mouseClickTypeBox.Items.Add("Right Up");
					mouseClickTypeBox.Items.Add("Double Left Click");

					if (sender != null)
					{
						actionParameters.Rows.Add("Click Type", "Left Click");
						actionParameters.Rows.Add("Click Position", "Center");
						actionParameters.Rows.Add("X Adjustment", 0);
						actionParameters.Rows.Add("Y Adjustment", 0);
					}

					_imageGridViewHelper.Rows[0].Cells[1] = mouseClickTypeBox;
					_imageGridViewHelper.Rows[1].Cells[1] = mouseClickPositionBox;

					break;

				case "Set Text":
					foreach (var ctrl in _imageParameterControls)
						ctrl.Show();

					DataGridViewComboBoxCell encryptedBox = new DataGridViewComboBoxCell();
					encryptedBox.Items.Add("Not Encrypted");
					encryptedBox.Items.Add("Encrypted");

					if (sender != null)
					{
						actionParameters.Rows.Add("Text To Set");
						actionParameters.Rows.Add("Click Position", "Center");
						actionParameters.Rows.Add("X Adjustment", 0);
						actionParameters.Rows.Add("Y Adjustment", 0);
						actionParameters.Rows.Add("Encrypted Text", "Not Encrypted");
						actionParameters.Rows.Add("Optional - Click to Encrypt 'Text To Set'");

						var buttonCell = new DataGridViewButtonCell();
						_imageGridViewHelper.Rows[5].Cells[1] = buttonCell;
						_imageGridViewHelper.Rows[5].Cells[1].Value = "Encrypt Text";
						_imageGridViewHelper.CellContentClick += ImageGridViewHelper_CellContentClick;
					}

					_imageGridViewHelper.Rows[1].Cells[1] = mouseClickPositionBox;
					_imageGridViewHelper.Rows[4].Cells[1] = encryptedBox;

					break;

				case "Set Secure Text":
					foreach (var ctrl in _imageParameterControls)
						ctrl.Show();

					if (sender != null)
					{
						actionParameters.Rows.Add("Secure String Variable");
						actionParameters.Rows.Add("Click Position", "Center");
						actionParameters.Rows.Add("X Adjustment", 0);
						actionParameters.Rows.Add("Y Adjustment", 0);
					}

					_imageGridViewHelper.Rows[1].Cells[1] = mouseClickPositionBox;

					break;

				case "Check If Image Exists":
					foreach (var ctrl in _imageParameterControls)
						ctrl.Show();

					if (sender != null)
						actionParameters.Rows.Add("Output Bool Variable Name", "");
					break;

				case "Wait For Image To Exist":
					foreach (var ctrl in _imageParameterControls)
						ctrl.Show();

					if (sender != null)
						actionParameters.Rows.Add("Timeout (Seconds)", 30);
					break;

				default:
					break;
			}
		}

		private void ImageGridViewHelper_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var targetCell = _imageGridViewHelper.Rows[e.RowIndex].Cells[e.ColumnIndex];

			if (targetCell is DataGridViewButtonCell && targetCell.Value.ToString() == "Encrypt Text")
			{
				var targetElement = _imageGridViewHelper.Rows[0].Cells[1];

				if (string.IsNullOrEmpty(targetElement.Value.ToString()))
					return;

				var warning = MessageBox.Show($"Warning! Text should only be encrypted one time and is not reversible in the builder. " +
											   "Would you like to proceed and convert '{targetElement.Value.ToString()}' to an encrypted value?",
											   "Encryption Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (warning == DialogResult.Yes)
				{
					targetElement.Value = EncryptionServices.EncryptString(targetElement.Value.ToString(), "OPENBOTS");
					_imageGridViewHelper.Rows[4].Cells[1].Value = "Encrypted";
				}
			}
		}

		private Point GetClickPosition(string clickPosition, ImageElement element)
		{
			int clickPositionX = 0;
			int clickPositionY = 0;
			switch (clickPosition)
			{
				case "Center":
					clickPositionX = element.MiddleX;
					clickPositionY = element.MiddleY;
					break;
				case "Top Left":
					clickPositionX = element.LeftX;
					clickPositionY = element.TopY;
					break;
				case "Top Middle":
					clickPositionX = element.MiddleX;
					clickPositionY = element.TopY;
					break;
				case "Top Right":
					clickPositionX = element.RightX;
					clickPositionY = element.TopY;
					break;
				case "Bottom Left":
					clickPositionX = element.LeftX;
					clickPositionY = element.BottomY;
					break;
				case "Bottom Middle":
					clickPositionX = element.MiddleX;
					clickPositionY = element.BottomY;
					break;
				case "Bottom Right":
					clickPositionX = element.RightX;
					clickPositionY = element.BottomY;
					break;
				case "Middle Left":
					clickPositionX = element.LeftX;
					clickPositionY = element.MiddleX;
					break;
				case "Middle Right":
					clickPositionX = element.RightX;
					clickPositionY = element.MiddleY;
					break;
			}
			return new Point(clickPositionX, clickPositionY);
		}
		private void ImageGridViewHelper_MouseEnter(object sender, EventArgs e)
		{
			ImageAction_SelectionChangeCommitted(null, null);
		}
	}
}