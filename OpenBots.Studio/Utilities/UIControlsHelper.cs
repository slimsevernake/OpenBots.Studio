using OpenBots.Core.Settings;
using OpenBots.Core.UI.Controls.CustomControls;
using OpenBots.Core.Utilities.CommandUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenBots.Utilities
{
    public static class UIControlsHelper
    {
        public static List<AutomationCommand> GenerateCommandsandControls()
        {
            var commandList = new List<AutomationCommand>();

            var commandClasses = Assembly.GetExecutingAssembly().GetTypes()
                                 .Where(t => t.Namespace == "OpenBots.Commands")
                                 .Where(t => t.Name != "ScriptCommand")
                                 .Where(t => t.IsAbstract == false)
                                 .Where(t => t.BaseType.Name == "ScriptCommand")
                                 .ToList();

            var cmdAssemblyPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Commands.*.dll");
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
                var newAutomationCommand = CommandsHelper.ConvertToAutomationCommand(commandClass);

                //If command is enabled, pull for display and configuration
                if (newAutomationCommand != null)
                    commandList.Add(newAutomationCommand);
            }

            return commandList;
        }
    }
}
