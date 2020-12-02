using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenBots.Core.Gallery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile;

namespace OpenBots.Core.Project
{
    public class Project
    {
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Main { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }

        [JsonIgnore]
        public List<string> DefaultCommands = new List<string>()
        {
            //"Data",
            //"DataTable",
            //"Dictionary",
            //"Email",
            "Engine",
            //"Excel",
            //"File",
            //"Folder",
            "If",
            "Input",
            //"List",
            //"Loop",
            "Misc",
            //"Process",
            //"RegEx",
            //"SecureData",
            //"Switch",
            //"System",
            //"TextFile",
            //"Variable",
            "WebBrowser",
            "Window",
        };

        public Project(string projectName)
        {
            ProjectID = Guid.NewGuid();
            ProjectName = projectName;
            Main = "Main.json";
            Version = Application.ProductVersion;
            Dependencies = new Dictionary<string, string>();

            foreach (string commandSet in DefaultCommands)
                Dependencies.Add($"OpenBots.Commands.{commandSet}", "1.1.0");
        }

        public void SaveProject(string scriptPath)
        {
            //Looks through sequential parent directories to find one that matches the script's ProjectName and contains a Main
            string projectPath;
            string dirName;
            string configPath;

            try
            {
                do
                {
                    projectPath = Path.GetDirectoryName(scriptPath);
                    DirectoryInfo dirInfo = new DirectoryInfo(projectPath);
                    dirName = dirInfo.Name;
                    configPath = Path.Combine(projectPath, "project.config");
                    scriptPath = projectPath;
                } while (dirName != ProjectName || !File.Exists(configPath));

                //If requirements are met, a project.config is created/updated
                if (dirName == ProjectName && File.Exists(configPath))
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(this));
            }
            catch (Exception)
            {
                throw new Exception("Project Directory Not Found. File Saved Externally");
            }                       
        }

        public static Project OpenProject(string configFilePath)
        {
            //Loads project from project.config
            if (File.Exists(configFilePath))
            {               
                string projectJSONString = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<Project>(projectJSONString);
            }
            else
            {
                throw new Exception("project.config Not Found");
            }
        }

        public static string ExtractGalleryProject(string projectDirectory)
        {
            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            var processNugetFilePath = projectDirectory + ".nupkg";
            var processZipFilePath = projectDirectory + ".zip";

            // Create .zip file
            File.Copy(processNugetFilePath, processZipFilePath, true);

            // Extract Files/Folders from (.zip) file
            DecompressFile(processZipFilePath, projectDirectory);

            // Delete .zip File
            File.Delete(processZipFilePath);
            File.Delete(processNugetFilePath);

            //get config file and rename project
            string configFilePath = Directory.GetFiles(projectDirectory, "project.config", SearchOption.AllDirectories).First();
            var config = JObject.Parse(File.ReadAllText(configFilePath));
            config["ProjectName"] = new DirectoryInfo(projectDirectory).Name;
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config));

            // Return "Main" Script File Path of the Process
            return configFilePath;
        }

        public static void DecompressFile(string projectFilePath, string targetDirectory)
        {
            FileStream fs = File.OpenRead(projectFilePath);
            ZipFile file = new ZipFile(fs);

            foreach (ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                {
                    // Ignore directories but create them in case they're empty
                    Directory.CreateDirectory(Path.Combine(targetDirectory, zipEntry.Name));
                    continue;
                }

                //exclude nuget metadata files
                string[] excludedFiles = { ".nuspec", ".xml", ".rels", ".psmdcp" };
                if (excludedFiles.Any(e => Path.GetExtension(zipEntry.Name) == e))
                    continue;

                string entryFileName = zipEntry.Name;

                // 4K is optimum
                byte[] buffer = new byte[4096];
                Stream zipStream = file.GetInputStream(zipEntry);

                // Manipulate the output filename here as desired.
                string fullZipToPath = Path.Combine(targetDirectory, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);

                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
            }

            if (file != null)
            {
                file.IsStreamOwner = true;
                file.Close();
            }
        }       
    }
}
