using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public static Dictionary<string, string> GetAgentSettings()
        {
            if (GetEnvironmentVariable() == null)
                throw new Exception("Agent environment variable not found");

            string agentSettingsPath = Path.Combine(GetEnvironmentVariable(), SettingsFileName);

            if (agentSettingsPath == null || !File.Exists(agentSettingsPath))
                throw new Exception("Agent settings file not found");

            string agentSettingsText = File.ReadAllText(agentSettingsPath);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(agentSettingsText);
            return settings;
        }
    }
}
