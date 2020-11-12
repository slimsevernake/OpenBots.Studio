using OpenBots.Core.Command;
using OpenBots.Core.Settings;
using OpenBots.UI.CustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace OpenBots.Utilities
{
    public static class UIControlsHelper
    {
        public static void ShowAllForms()
        {
            foreach (Form form in Application.OpenForms)
                ShowForm(form);

            Thread.Sleep(1000);
        }

        public delegate void ShowFormDelegate(Form form);
        public static void ShowForm(Form form)
        {
            if (form.InvokeRequired)
            {
                var d = new ShowFormDelegate(ShowForm);
                form.Invoke(d, new object[] { form });
            }
            else
                form.WindowState = FormWindowState.Normal;
        }

        public static void HideAllForms()
        {
            foreach (Form form in Application.OpenForms)
                HideForm(form);

            Thread.Sleep(1000);
        }

        public delegate void HideFormDelegate(Form form);
        public static void HideForm(Form form)
        {
            if (form.InvokeRequired)
            {
                var d = new HideFormDelegate(HideForm);
                form.Invoke(d, new object[] { form });
            }
            else
                form.WindowState = FormWindowState.Minimized;
        }

        public static List<AutomationCommand> GenerateCommandsandControls(Dictionary<string,string> dependencies = null) 
        {
            var commandList = new List<AutomationCommand>();

            var commandClasses = Assembly.GetExecutingAssembly().GetTypes()
                                 .Where(t => t.Namespace == "OpenBots.Commands")
                                 .Where(t => t.Name != "ScriptCommand")
                                 .Where(t => t.IsAbstract == false)
                                 .Where(t => t.BaseType.Name == "ScriptCommand")
                                 .ToList();

            var cmdAssemblyPaths = new List<string>();//Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Commands.*.dll").ToList();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string packagePath = Path.Combine(appDataPath, "OpenBots Inc", "packages");
            List<string> commandDirectories = Directory.GetDirectories(packagePath, "OpenBots.Commands.*").ToList();
            List<string> filteredCommandDirectories = new List<string>();
            
            foreach (var pair in dependencies)
            {
                foreach(string directory in commandDirectories)
                {
                    if (new DirectoryInfo(directory).Name == $"{pair.Key}.{pair.Value}")
                        filteredCommandDirectories.Add(directory);
                }
            }

            foreach (string directory in filteredCommandDirectories)
                cmdAssemblyPaths.AddRange(Directory.GetFiles(Path.Combine(directory, "lib", "net48"), "OpenBots.Commands.*.dll").ToList());

            foreach (var path in cmdAssemblyPaths)
            {
                commandClasses.AddRange(Assembly.LoadFrom(path).GetTypes()
                                 .Where(t => t.Name != "ScriptCommand")
                                 .Where(t => t.IsAbstract == false)
                                 .Where(t => t.BaseType.Name == "ScriptCommand")
                                 .ToList());
            }
            var userPrefs = new ApplicationSettings().GetOrCreateApplicationSettings();

            //Loop through each class
            foreach (var commandClass in commandClasses)
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

                //If command is enabled, pull for display and configuration
                if (newCommand.CommandEnabled)
                {
                    var newAutomationCommand = new AutomationCommand();
                    newAutomationCommand.CommandClass = commandClass;
                    newAutomationCommand.Command = newCommand;
                    newAutomationCommand.DisplayGroup = groupAttribute;
                    newAutomationCommand.FullName = string.Join(" - ", groupAttribute, newCommand.SelectionName);
                    newAutomationCommand.ShortName = newCommand.SelectionName;
                    newAutomationCommand.Description = GetDescription(commandClass);

                    if (userPrefs.ClientSettings.PreloadBuilderCommands)
                    {
                        //newAutomationCommand.RenderUIComponents();
                    }

                    //call RenderUIComponents to render UI controls
                    commandList.Add(newAutomationCommand);
                }
            }

            return commandList;
        }

        static string GetDescription(Type type)
        {
            var descriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), true);

            if (descriptions.Length == 0)
                return string.Empty;
            return descriptions[0].Description;
        }
    }
}
