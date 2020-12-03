using Newtonsoft.Json;
using OpenBots.Core.Enums;
using OpenBots.Core.IO;
using System;
using System.Data;
using System.IO;

namespace OpenBots.Core.Settings
{
    /// <summary>
    /// Defines settings for the entire application
    /// </summary>
    [Serializable]
    public class ApplicationSettings
    {
        public EngineSettings EngineSettings { get; set; }
        public ClientSettings ClientSettings { get; set; }

        public ApplicationSettings()
        {
            EngineSettings = new EngineSettings();
            ClientSettings = new ClientSettings();
        }

        public void Save(ApplicationSettings appSettings)
        {
            //create settings directory
            var settingsDir = Folders.GetFolder(FolderType.SettingsFolder);

            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            //create file path
            var filePath = Path.Combine(settingsDir, "AppSettings.json");

            var serializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            JsonSerializer serializer = JsonSerializer.Create(serializerSettings);

            //output to json file
            //if output path was provided
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw){ Formatting = Formatting.Indented })
            {
                serializer.Serialize(writer, appSettings, typeof(ApplicationSettings));
            }
        }

        public ApplicationSettings GetOrCreateApplicationSettings()
        {
            //create settings directory
            var settingsDir = Folders.GetFolder(FolderType.SettingsFolder);

            //create file path
            var filePath = Path.Combine(settingsDir, "AppSettings.json");

            ApplicationSettings appSettings;
            if (File.Exists(filePath))
            {
                //open file and return it or return new settings on error
                try
                {
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        var serializerSettings = new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.Objects                            
                        };

                        JsonSerializer serializer = JsonSerializer.Create(serializerSettings);
                        appSettings = (ApplicationSettings)serializer.Deserialize(file, typeof(ApplicationSettings));
                    }
                }
                catch (Exception)
                {
                    appSettings = new ApplicationSettings();
                    appSettings.EngineSettings.PackageSourceDT = DefaultPackageSourceDT();
                }
            }
            else
            {
                appSettings = new ApplicationSettings();
                appSettings.EngineSettings.PackageSourceDT = DefaultPackageSourceDT();
            }

            return appSettings;
        }

        private DataTable DefaultPackageSourceDT()
        {
            DataTable packageSourceDT = new DataTable();
            packageSourceDT.Columns.Add("Enabled");
            packageSourceDT.Columns.Add("Package Name");
            packageSourceDT.Columns.Add("Package Source");
            packageSourceDT.TableName = DateTime.Now.ToString("PackageSourceDT" + DateTime.Now.ToString("MMddyy.hhmmss"));
            packageSourceDT.Rows.Add(true, "Gallery", "https://dev.gallery.openbots.io/v3/index.json");
            packageSourceDT.Rows.Add(true, "Nuget", "https://api.nuget.org/v3/index.json");

            return packageSourceDT;
        }
    }
}
