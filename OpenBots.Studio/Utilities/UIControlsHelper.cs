using Autofac;
using OpenBots.Core.Command;
using OpenBots.Core.Settings;
using OpenBots.Core.UI.Controls.CustomControls;
using OpenBots.Core.Utilities.CommandUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenBots.Utilities
{
    public static class UIControlsHelper
    {       
        public static List<AutomationCommand> GenerateCommandsandControls(Autofac.IContainer container = null)
        {
            var commandList = new List<AutomationCommand>();
            var commandClasses = new List<Type>();
           
            //TODO delete query when commands are fully removed from studio
            commandClasses = Assembly.GetExecutingAssembly().GetTypes()
                                 .Where(t => t.Namespace == "OpenBots.Commands")
                                 .Where(t => t.Name != "ScriptCommand")
                                 .Where(t => t.IsAbstract == false)
                                 .Where(t => t.BaseType.Name == "ScriptCommand")
                                 .ToList();

            using (var scope = container.BeginLifetimeScope())
            {
                    var types = scope.ComponentRegistry.Registrations
                                .Where(r => typeof(ScriptCommand).IsAssignableFrom(r.Activator.LimitType))
                                .Select(r => r.Activator.LimitType).ToList();

                    commandClasses.AddRange(types);
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
