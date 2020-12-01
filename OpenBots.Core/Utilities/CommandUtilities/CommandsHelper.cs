using OpenBots.Core.Command;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.UI.Controls.CustomControls;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Automation;

namespace OpenBots.Core.Utilities.CommandUtilities
{
    public static class CommandsHelper
    {
        public static AutomationCommand ConvertToAutomationCommand(Type commandClass)
        {
            var groupingAttribute = commandClass.GetCustomAttributes(typeof(CategoryAttribute), true);
            string groupAttribute = "";
            if (groupingAttribute.Length > 0)
            {
                var attributeFound = (CategoryAttribute)groupingAttribute[0];
                groupAttribute = attributeFound.Category;
            }

            //Instantiate Class
            ScriptCommand newCommand = (ScriptCommand)Activator.CreateInstance(commandClass);

            AutomationCommand newAutomationCommand = null;
            //If command is enabled, pull for display and configuration
            if (newCommand.CommandEnabled)
            {
                newAutomationCommand = new AutomationCommand();
                newAutomationCommand.CommandClass = commandClass;
                newAutomationCommand.Command = newCommand;
                newAutomationCommand.DisplayGroup = groupAttribute;
                newAutomationCommand.FullName = string.Join(" - ", groupAttribute, newCommand.SelectionName);
                newAutomationCommand.ShortName = newCommand.SelectionName;
                newAutomationCommand.Description = GetDescription(commandClass);

                //if (userPrefs.ClientSettings.PreloadBuilderCommands)
                //{
                //    //newAutomationCommand.RenderUIComponents();
                //}
            }

            return newAutomationCommand;
        }
        private static string GetDescription(Type type)
        {
            var descriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), true);

            if (descriptions.Length == 0)
                return string.Empty;
            return descriptions[0].Description;
        }

        public static AutomationElement SearchForGUIElement(IEngine engine, DataTable uiaSearchParams, string variableWindowName)
        {
            //create search params
            var searchParams = from rw in uiaSearchParams.AsEnumerable()
                               where rw.Field<string>("Enabled") == "True"
                               select rw;

            //create and populate condition list
            var conditionList = new List<Condition>();
            foreach (var param in searchParams)
            {
                var parameterName = (string)param["Parameter Name"];
                var parameterValue = (string)param["Parameter Value"];

                parameterValue = parameterValue.ConvertUserVariableToString(engine);

                PropertyCondition propCondition;
                if (bool.TryParse(parameterValue, out bool bValue))
                    propCondition = CreatePropertyCondition(parameterName, bValue);
                else
                    propCondition = CreatePropertyCondition(parameterName, parameterValue);

                conditionList.Add(propCondition);
            }

            //concatenate or take first condition
            Condition searchConditions;
            if (conditionList.Count > 1)
                searchConditions = new AndCondition(conditionList.ToArray());
            else
                searchConditions = conditionList[0];

            //find window
            var windowElement = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, variableWindowName));

            //if window was not found
            if (windowElement == null)
                throw new Exception("Window named '" + variableWindowName + "' was not found!");

            //find required handle based on specified conditions
            var element = windowElement.FindFirst(TreeScope.Descendants, searchConditions);
            return element;
        }
        private static PropertyCondition CreatePropertyCondition(string propertyName, object propertyValue)
        {
            switch (propertyName)
            {
                case "AcceleratorKey":
                    return new PropertyCondition(AutomationElement.AcceleratorKeyProperty, propertyValue);
                case "AccessKey":
                    return new PropertyCondition(AutomationElement.AccessKeyProperty, propertyValue);
                case "AutomationId":
                    return new PropertyCondition(AutomationElement.AutomationIdProperty, propertyValue);
                case "ClassName":
                    return new PropertyCondition(AutomationElement.ClassNameProperty, propertyValue);
                case "FrameworkId":
                    return new PropertyCondition(AutomationElement.FrameworkIdProperty, propertyValue);
                case "HasKeyboardFocus":
                    return new PropertyCondition(AutomationElement.HasKeyboardFocusProperty, propertyValue);
                case "HelpText":
                    return new PropertyCondition(AutomationElement.HelpTextProperty, propertyValue);
                case "IsContentElement":
                    return new PropertyCondition(AutomationElement.IsContentElementProperty, propertyValue);
                case "IsControlElement":
                    return new PropertyCondition(AutomationElement.IsControlElementProperty, propertyValue);
                case "IsEnabled":
                    return new PropertyCondition(AutomationElement.IsEnabledProperty, propertyValue);
                case "IsKeyboardFocusable":
                    return new PropertyCondition(AutomationElement.IsKeyboardFocusableProperty, propertyValue);
                case "IsOffscreen":
                    return new PropertyCondition(AutomationElement.IsOffscreenProperty, propertyValue);
                case "IsPassword":
                    return new PropertyCondition(AutomationElement.IsPasswordProperty, propertyValue);
                case "IsRequiredForForm":
                    return new PropertyCondition(AutomationElement.IsRequiredForFormProperty, propertyValue);
                case "ItemStatus":
                    return new PropertyCondition(AutomationElement.ItemStatusProperty, propertyValue);
                case "ItemType":
                    return new PropertyCondition(AutomationElement.ItemTypeProperty, propertyValue);
                case "LocalizedControlType":
                    return new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, propertyValue);
                case "Name":
                    return new PropertyCondition(AutomationElement.NameProperty, propertyValue);
                case "NativeWindowHandle":
                    return new PropertyCondition(AutomationElement.NativeWindowHandleProperty, propertyValue);
                case "ProcessID":
                    return new PropertyCondition(AutomationElement.ProcessIdProperty, propertyValue);
                default:
                    throw new NotImplementedException("Property Type '" + propertyName + "' not implemented");
            }
        }

        public static bool ElementExists(IEngine engine, string instanceName, string searchMethod, string parameterName, 
            string searchOption, int timeout)
        {
            //get engine reference
            List<string[]> seleniumSearchParamRows = new List<string[]>();
            seleniumSearchParamRows.Add(new string[]
            {
                string.Empty, searchMethod, parameterName
            });

            //get stored app object
            var browserObject = instanceName.GetAppInstance(engine);

            //get selenium instance driver
            var seleniumInstance = (ChromeDriver)browserObject;

            try
            {
                //search for element
                var element = FindElement(engine, seleniumInstance, seleniumSearchParamRows, searchOption, timeout);

                //element exists
                return true;
            }
            catch (Exception)
            {
                //element does not exist
                return false;
            }
        }

        public static object FindElement(IEngine engine, IWebDriver seleniumInstance, List<string[]> searchParameterRows, 
            string searchOption, int timeout)
        {
            var wait = new WebDriverWait(seleniumInstance, new TimeSpan(0, 0, timeout));
            object element;

            List<By> byList = new List<By>();
            By by;

            foreach (var row in searchParameterRows)
            {
                string parameter = row[2].ToString().ConvertUserVariableToString(engine);
                switch (row[1].ToString())
                {
                    case string a when a.ToLower().Contains("xpath"):
                        by = By.XPath(parameter);
                        break;

                    case string a when a.ToLower().Contains("id"):
                        by = By.Id(parameter);
                        break;

                    case string a when a.ToLower().Contains("tag name"):
                        by = By.TagName(parameter);
                        break;

                    case string a when a.ToLower().Contains("class name"):
                        by = By.ClassName(parameter);
                        break;

                    case string a when a.ToLower().Contains("name"):
                        by = By.Name(parameter);
                        break;

                    case string a when a.ToLower().Contains("css selector"):
                        by = By.CssSelector(parameter);
                        break;

                    case string a when a.ToLower().Contains("link text"):
                        by = By.LinkText(parameter);
                        break;

                    default:
                        throw new Exception("Element Search Type was not found: " + row[1].ToString());
                }
                byList.Add(by);
            }

            var byall = new ByAll(byList.ToArray());
            bool elementFound;

            if (searchOption == "Find Element")
            {
                try
                {
                    elementFound = wait.Until(condition =>
                    {
                        try
                        {
                            var elementToBeDisplayed = seleniumInstance.FindElement(byall);
                            return elementToBeDisplayed.Displayed;
                        }
                        catch (StaleElementReferenceException)
                        {
                            return false;
                        }
                        catch (NoSuchElementException)
                        {
                            return false;
                        }
                    });
                }
                catch (Exception)
                {
                    //element not found during wait period
                }

                element = seleniumInstance.FindElement(byall);
            }
            else
            {
                try
                {
                    elementFound = wait.Until(condition =>
                    {
                        try
                        {
                            var elementsToBeDisplayed = seleniumInstance.FindElements(byall);
                            return elementsToBeDisplayed.First().Displayed;
                        }
                        catch (StaleElementReferenceException)
                        {
                            return false;
                        }
                        catch (NoSuchElementException)
                        {
                            return false;
                        }
                    });
                }
                catch (Exception)
                {
                    //elements not found during wait period
                }

                element = seleniumInstance.FindElements(byall);
            }

            return element;
        }
    }
}
