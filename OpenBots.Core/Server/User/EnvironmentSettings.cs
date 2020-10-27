using System;
using System.IO;

namespace OpenBots.Core.Server.User
{
    public class EnvironmentSettings
    {
        public static string EnvironmentVariableName { get; } = "OpenBots_Agent_Data_Path";
        public static string EnvironmentVariableValue { get; } = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "OpenBots Inc",
                        "OpenBots Agent"
                        );
        public static string SettingsFileName { get; } = "OpenBots.settings";

        public static string GetEnvironmentVariable()
        {
            return Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.Machine);
        }
    }
}
