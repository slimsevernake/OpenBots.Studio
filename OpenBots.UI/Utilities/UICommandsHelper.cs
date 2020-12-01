using OpenBots.Core.Infrastructure;
using OpenBots.Core.User32;
using OpenBots.Core.Utilities.CommandUtilities;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Core.Utilities.FormsUtilities;
using OpenBots.UI.SupplementForms;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace OpenBots.UI.Utilities
{
	public static class UICommandsHelper
    {
		public static ImageElement FindImageElement(Bitmap smallBmp, double accuracy, bool isTestMode)
		{
			FormsHelper.HideAllForms();
			
			dynamic element = null;
			double tolerance = 1.0 - accuracy;

			Bitmap bigBmp = ImageMethods.Screenshot();

			Bitmap smallTestBmp = new Bitmap(smallBmp);

			Bitmap bigTestBmp = new Bitmap(bigBmp);
			Graphics bigTestGraphics = Graphics.FromImage(bigTestBmp);

			BitmapData smallData =
			  smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height),
					   ImageLockMode.ReadOnly,
					   PixelFormat.Format24bppRgb);
			BitmapData bigData =
			  bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height),
					   ImageLockMode.ReadOnly,
					   PixelFormat.Format24bppRgb);

			int smallStride = smallData.Stride;
			int bigStride = bigData.Stride;

			int bigWidth = bigBmp.Width;
			int bigHeight = bigBmp.Height - smallBmp.Height + 1;
			int smallWidth = smallBmp.Width * 3;
			int smallHeight = smallBmp.Height;

			int margin = Convert.ToInt32(255.0 * tolerance);

			unsafe
			{
				byte* pSmall = (byte*)(void*)smallData.Scan0;
				byte* pBig = (byte*)(void*)bigData.Scan0;

				int smallOffset = smallStride - smallBmp.Width * 3;
				int bigOffset = bigStride - bigBmp.Width * 3;

				bool matchFound = true;

				for (int y = 0; y < bigHeight; y++)
				{
					for (int x = 0; x < bigWidth; x++)
					{
						byte* pBigBackup = pBig;
						byte* pSmallBackup = pSmall;

						//Look for the small picture.
						for (int i = 0; i < smallHeight; i++)
						{
							int j = 0;
							matchFound = true;
							for (j = 0; j < smallWidth; j++)
							{
								//With tolerance: pSmall value should be between margins.
								int inf = pBig[0] - margin;
								int sup = pBig[0] + margin;
								if (sup < pSmall[0] || inf > pSmall[0])
								{
									matchFound = false;
									break;
								}

								pBig++;
								pSmall++;
							}

							if (!matchFound)
								break;

							//We restore the pointers.
							pSmall = pSmallBackup;
							pBig = pBigBackup;

							//Next rows of the small and big pictures.
							pSmall += smallStride * (1 + i);
							pBig += bigStride * (1 + i);
						}

						//If match found, we return.
						if (matchFound)
						{
							element = new ImageElement
							{
								LeftX = x,
								MiddleX = x + smallBmp.Width / 2,
								RightX = x + smallBmp.Width,
								TopY = y,
								MiddleY = y + smallBmp.Height / 2,
								BottomY = y + smallBmp.Height
							};

							if (isTestMode)
							{
								//draw on output to demonstrate finding
								var Rectangle = new Rectangle(x, y, smallBmp.Width - 1, smallBmp.Height - 1);
								Pen pen = new Pen(Color.Red);
								pen.Width = 5.0F;
								bigTestGraphics.DrawRectangle(pen, Rectangle);

								frmImageCapture captureOutput = new frmImageCapture();
								captureOutput.pbTaggedImage.Image = smallTestBmp;
								captureOutput.pbSearchResult.Image = bigTestBmp;
								captureOutput.TopMost = true;
								captureOutput.Show();
							}
							break;
						}
						//If no match found, we restore the pointers and continue.
						else
						{
							pBig = pBigBackup;
							pSmall = pSmallBackup;
							pBig += 3;
						}
					}

					if (matchFound)
						break;

					pBig += bigOffset;
				}
			}

			bigBmp.UnlockBits(bigData);
			smallBmp.UnlockBits(smallData);
			bigTestGraphics.Dispose();
			return element;
		}

		public static bool DetermineStatementTruth(IEngine engine, string ifActionType, DataTable IfActionParameterTable)
		{
			bool ifResult = false;

			if (ifActionType == "Value Compare")
			{
				string value1 = ((from rw in IfActionParameterTable.AsEnumerable()
								  where rw.Field<string>("Parameter Name") == "Value1"
								  select rw.Field<string>("Parameter Value")).FirstOrDefault());
				string operand = ((from rw in IfActionParameterTable.AsEnumerable()
								   where rw.Field<string>("Parameter Name") == "Operand"
								   select rw.Field<string>("Parameter Value")).FirstOrDefault());
				string value2 = ((from rw in IfActionParameterTable.AsEnumerable()
								  where rw.Field<string>("Parameter Name") == "Value2"
								  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				value1 = value1.ConvertUserVariableToString(engine);
				value2 = value2.ConvertUserVariableToString(engine);

				decimal cdecValue1, cdecValue2;

				switch (operand)
				{
					case "is equal to":
						ifResult = (value1 == value2);
						break;

					case "is not equal to":
						ifResult = (value1 != value2);
						break;

					case "is greater than":
						cdecValue1 = Convert.ToDecimal(value1);
						cdecValue2 = Convert.ToDecimal(value2);
						ifResult = (cdecValue1 > cdecValue2);
						break;

					case "is greater than or equal to":
						cdecValue1 = Convert.ToDecimal(value1);
						cdecValue2 = Convert.ToDecimal(value2);
						ifResult = (cdecValue1 >= cdecValue2);
						break;

					case "is less than":
						cdecValue1 = Convert.ToDecimal(value1);
						cdecValue2 = Convert.ToDecimal(value2);
						ifResult = (cdecValue1 < cdecValue2);
						break;

					case "is less than or equal to":
						cdecValue1 = Convert.ToDecimal(value1);
						cdecValue2 = Convert.ToDecimal(value2);
						ifResult = (cdecValue1 <= cdecValue2);
						break;
				}
			}
			else if (ifActionType == "Date Compare")
			{
				string value1 = ((from rw in IfActionParameterTable.AsEnumerable()
								  where rw.Field<string>("Parameter Name") == "Value1"
								  select rw.Field<string>("Parameter Value")).FirstOrDefault());
				string operand = ((from rw in IfActionParameterTable.AsEnumerable()
								   where rw.Field<string>("Parameter Name") == "Operand"
								   select rw.Field<string>("Parameter Value")).FirstOrDefault());
				string value2 = ((from rw in IfActionParameterTable.AsEnumerable()
								  where rw.Field<string>("Parameter Name") == "Value2"
								  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				DateTime dt1, dt2;

				dynamic input1 = value1.ConvertUserVariableToString(engine);

				if (input1 == value1 && input1.StartsWith("{") && input1.EndsWith("}"))
					input1 = value1.ConvertUserVariableToObject(engine);

				if (input1 is DateTime)
					dt1 = (DateTime)input1;
				else if (input1 is string)
					dt1 = DateTime.Parse((string)input1);
				else
					throw new InvalidDataException("Value1 is not a valid DateTime");

				dynamic input2 = value2.ConvertUserVariableToString(engine);

				if (input2 == value2 && input2.StartsWith("{") && input2.EndsWith("}"))
					input2 = value2.ConvertUserVariableToObject(engine);

				if (input2 is DateTime)
					dt2 = (DateTime)input2;
				else if (input2 is string)
					dt2 = DateTime.Parse((string)input2);
				else
					throw new InvalidDataException("Value2 is not a valid DateTime");

				switch (operand)
				{
					case "is equal to":
						ifResult = (dt1 == dt2);
						break;

					case "is not equal to":
						ifResult = (dt1 != dt2);
						break;

					case "is greater than":
						ifResult = (dt1 > dt2);
						break;

					case "is greater than or equal to":
						ifResult = (dt1 >= dt2);
						break;

					case "is less than":
						ifResult = (dt1 < dt2);
						break;

					case "is less than or equal to":
						ifResult = (dt1 <= dt2);
						break;
				}
			}
			else if (ifActionType == "Variable Compare")
			{
				string value1 = ((from rw in IfActionParameterTable.AsEnumerable()
								  where rw.Field<string>("Parameter Name") == "Value1"
								  select rw.Field<string>("Parameter Value")).FirstOrDefault());
				string operand = ((from rw in IfActionParameterTable.AsEnumerable()
								   where rw.Field<string>("Parameter Name") == "Operand"
								   select rw.Field<string>("Parameter Value")).FirstOrDefault());
				string value2 = ((from rw in IfActionParameterTable.AsEnumerable()
								  where rw.Field<string>("Parameter Name") == "Value2"
								  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				string caseSensitive = ((from rw in IfActionParameterTable.AsEnumerable()
										 where rw.Field<string>("Parameter Name") == "Case Sensitive"
										 select rw.Field<string>("Parameter Value")).FirstOrDefault());

				value1 = value1.ConvertUserVariableToString(engine);
				value2 = value2.ConvertUserVariableToString(engine);

				if (caseSensitive == "No")
				{
					value1 = value1.ToUpper();
					value2 = value2.ToUpper();
				}

				switch (operand)
				{
					case "contains":
						ifResult = (value1.Contains(value2));
						break;

					case "does not contain":
						ifResult = (!value1.Contains(value2));
						break;

					case "is equal to":
						ifResult = (value1 == value2);
						break;

					case "is not equal to":
						ifResult = (value1 != value2);
						break;
				}
			}
			else if (ifActionType == "Variable Has Value")
			{
				string variableName = ((from rw in IfActionParameterTable.AsEnumerable()
										where rw.Field<string>("Parameter Name") == "Variable Name"
										select rw.Field<string>("Parameter Value")).FirstOrDefault());

				var actualVariable = variableName.ConvertUserVariableToObject(engine);

				if (actualVariable != null)
					ifResult = true;
				else
					ifResult = false;
			}
			else if (ifActionType == "Variable Is Numeric")
			{
				string variableName = ((from rw in IfActionParameterTable.AsEnumerable()
										where rw.Field<string>("Parameter Name") == "Variable Name"
										select rw.Field<string>("Parameter Value")).FirstOrDefault());

				var actualVariable = variableName.ConvertUserVariableToString(engine).Trim();

				var numericTest = decimal.TryParse(actualVariable, out decimal parsedResult);

				if (numericTest)
					ifResult = true;
				else
					ifResult = false;
			}
			else if (ifActionType == "Error Occured")
			{
				//get line number
				string userLineNumber = ((from rw in IfActionParameterTable.AsEnumerable()
										  where rw.Field<string>("Parameter Name") == "Line Number"
										  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				//convert to variable
				string variableLineNumber = userLineNumber.ConvertUserVariableToString(engine);

				//convert to int
				int lineNumber = int.Parse(variableLineNumber);

				//determine if error happened
				if (engine.ErrorsOccured.Where(f => f.LineNumber == lineNumber).Count() > 0)
				{

					var error = engine.ErrorsOccured.Where(f => f.LineNumber == lineNumber).FirstOrDefault();
					error.ErrorMessage.StoreInUserVariable(engine, "Error.Message");
					error.LineNumber.ToString().StoreInUserVariable(engine, "Error.Line");
					error.StackTrace.StoreInUserVariable(engine, "Error.StackTrace");

					ifResult = true;
				}
				else
					ifResult = false;
			}
			else if (ifActionType == "Error Did Not Occur")
			{
				//get line number
				string userLineNumber = ((from rw in IfActionParameterTable.AsEnumerable()
										  where rw.Field<string>("Parameter Name") == "Line Number"
										  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				//convert to variable
				string variableLineNumber = userLineNumber.ConvertUserVariableToString(engine);

				//convert to int
				int lineNumber = int.Parse(variableLineNumber);

				//determine if error happened
				if (engine.ErrorsOccured.Where(f => f.LineNumber == lineNumber).Count() == 0)
				{
					ifResult = true;
				}
				else
				{
					var error = engine.ErrorsOccured.Where(f => f.LineNumber == lineNumber).FirstOrDefault();
					error.ErrorMessage.StoreInUserVariable(engine, "Error.Message");
					error.LineNumber.ToString().StoreInUserVariable(engine, "Error.Line");
					error.StackTrace.StoreInUserVariable(engine, "Error.StackTrace");

					ifResult = false;
				}
			}
			else if (ifActionType == "Window Name Exists")
			{
				//get user supplied name
				string windowName = ((from rw in IfActionParameterTable.AsEnumerable()
									  where rw.Field<string>("Parameter Name") == "Window Name"
									  select rw.Field<string>("Parameter Value")).FirstOrDefault());
				//variable translation
				string variablizedWindowName = windowName.ConvertUserVariableToString(engine);

				//search for window
				IntPtr windowPtr = User32Functions.FindWindow(variablizedWindowName);

				//conditional
				if (windowPtr != IntPtr.Zero)
				{
					ifResult = true;
				}
			}
			else if (ifActionType == "Active Window Name Is")
			{
				string windowName = ((from rw in IfActionParameterTable.AsEnumerable()
									  where rw.Field<string>("Parameter Name") == "Window Name"
									  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				string variablizedWindowName = windowName.ConvertUserVariableToString(engine);

				var currentWindowTitle = User32Functions.GetActiveWindowTitle();

				if (currentWindowTitle == variablizedWindowName)
				{
					ifResult = true;
				}

			}
			else if (ifActionType == "File Exists")
			{

				string fileName = ((from rw in IfActionParameterTable.AsEnumerable()
									where rw.Field<string>("Parameter Name") == "File Path"
									select rw.Field<string>("Parameter Value")).FirstOrDefault());

				string trueWhenFileExists = ((from rw in IfActionParameterTable.AsEnumerable()
											  where rw.Field<string>("Parameter Name") == "True When"
											  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				var userFileSelected = fileName.ConvertUserVariableToString(engine);

				bool existCheck = false;
				if (trueWhenFileExists == "It Does Exist")
				{
					existCheck = true;
				}

				if (File.Exists(userFileSelected) == existCheck)
				{
					ifResult = true;
				}
			}
			else if (ifActionType == "Folder Exists")
			{
				string folderName = ((from rw in IfActionParameterTable.AsEnumerable()
									  where rw.Field<string>("Parameter Name") == "Folder Path"
									  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				string trueWhenFileExists = ((from rw in IfActionParameterTable.AsEnumerable()
											  where rw.Field<string>("Parameter Name") == "True When"
											  select rw.Field<string>("Parameter Value")).FirstOrDefault());

				var userFolderSelected = folderName.ConvertUserVariableToString(engine);

				bool existCheck = false;
				if (trueWhenFileExists == "It Does Exist")
				{
					existCheck = true;
				}

				if (Directory.Exists(userFolderSelected) == existCheck)
				{
					ifResult = true;
				}
			}
			else if (ifActionType == "Web Element Exists")
			{
				string instanceName = ((from rw in IfActionParameterTable.AsEnumerable()
										where rw.Field<string>("Parameter Name") == "Selenium Instance Name"
										select rw.Field<string>("Parameter Value")).FirstOrDefault());

				string parameterName = ((from rw in IfActionParameterTable.AsEnumerable()
										 where rw.Field<string>("Parameter Name") == "Element Search Parameter"
										 select rw.Field<string>("Parameter Value")).FirstOrDefault());

				string searchMethod = ((from rw in IfActionParameterTable.AsEnumerable()
										where rw.Field<string>("Parameter Name") == "Element Search Method"
										select rw.Field<string>("Parameter Value")).FirstOrDefault());

				string timeout = ((from rw in IfActionParameterTable.AsEnumerable()
								   where rw.Field<string>("Parameter Name") == "Timeout (Seconds)"
								   select rw.Field<string>("Parameter Value")).FirstOrDefault());

				string trueWhenElementExists = (from rw in IfActionParameterTable.AsEnumerable()
												where rw.Field<string>("Parameter Name") == "True When"
												select rw.Field<string>("Parameter Value")).FirstOrDefault();

				bool elementExists = CommandsHelper.ElementExists(engine, instanceName, searchMethod, parameterName, "Find Element", int.Parse(timeout));
				ifResult = elementExists;

				if (trueWhenElementExists == "It Does Not Exist")
					ifResult = !ifResult;
			}
			else if (ifActionType == "GUI Element Exists")
			{
				string windowName = ((from rw in IfActionParameterTable.AsEnumerable()
									  where rw.Field<string>("Parameter Name") == "Window Name"
									  select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));

				string elementSearchParam = ((from rw in IfActionParameterTable.AsEnumerable()
											  where rw.Field<string>("Parameter Name") == "Element Search Parameter"
											  select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));

				string elementSearchMethod = ((from rw in IfActionParameterTable.AsEnumerable()
											   where rw.Field<string>("Parameter Name") == "Element Search Method"
											   select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));

				string trueWhenElementExists = (from rw in IfActionParameterTable.AsEnumerable()
												where rw.Field<string>("Parameter Name") == "True When"
												select rw.Field<string>("Parameter Value")).FirstOrDefault();

				//set up search parameter table
				var uiASearchParameters = new DataTable();
				uiASearchParameters.Columns.Add("Enabled");
				uiASearchParameters.Columns.Add("Parameter Name");
				uiASearchParameters.Columns.Add("Parameter Value");
				uiASearchParameters.Rows.Add(true, elementSearchMethod, elementSearchParam);
				var handle = CommandsHelper.SearchForGUIElement(engine, uiASearchParameters, windowName);

				if (handle is null)
					ifResult = false;
				else
					ifResult = true;

				if (trueWhenElementExists == "It Does Not Exist")
					ifResult = !ifResult;
			}
			else if (ifActionType == "Image Element Exists")
			{
				string imageName = (from rw in IfActionParameterTable.AsEnumerable()
									where rw.Field<string>("Parameter Name") == "Captured Image Variable"
									select rw.Field<string>("Parameter Value")).FirstOrDefault();
				double accuracy;
				try
				{
					accuracy = double.Parse((from rw in IfActionParameterTable.AsEnumerable()
											 where rw.Field<string>("Parameter Name") == "Accuracy (0-1)"
											 select rw.Field<string>("Parameter Value")).FirstOrDefault().ConvertUserVariableToString(engine));
					if (accuracy > 1 || accuracy < 0)
						throw new ArgumentOutOfRangeException("Accuracy value is out of range (0-1)");
				}
				catch (Exception)
				{
					throw new InvalidDataException("Accuracy value is invalid");
				}

				string trueWhenImageExists = (from rw in IfActionParameterTable.AsEnumerable()
											  where rw.Field<string>("Parameter Name") == "True When"
											  select rw.Field<string>("Parameter Value")).FirstOrDefault();

				var imageVariable = imageName.ConvertUserVariableToObject(engine);

				Bitmap capturedImage;
				if (imageVariable != null && imageVariable is Bitmap)
					capturedImage = (Bitmap)imageVariable;
				else
					throw new ArgumentException("Provided Argument is not a 'Bitmap' Image");

				var element = FindImageElement(capturedImage, accuracy, false);
				FormsHelper.ShowAllForms();

				if (element != null)
					ifResult = true;
				else
					ifResult = false;

				if (trueWhenImageExists == "It Does Not Exist")
					ifResult = !ifResult;
			}
			else if (ifActionType == "App Instance Exists")
			{
				string instanceName = (from rw in IfActionParameterTable.AsEnumerable()
									   where rw.Field<string>("Parameter Name") == "Instance Name"
									   select rw.Field<string>("Parameter Value")).FirstOrDefault();


				string trueWhenImageExists = (from rw in IfActionParameterTable.AsEnumerable()
											  where rw.Field<string>("Parameter Name") == "True When"
											  select rw.Field<string>("Parameter Value")).FirstOrDefault();

				ifResult = instanceName.InstanceExists(engine);

				if (trueWhenImageExists == "It Does Not Exist")
					ifResult = !ifResult;
			}
			else
			{
				throw new Exception("If type not recognized!");
			}

			return ifResult;
		}


	}
}
