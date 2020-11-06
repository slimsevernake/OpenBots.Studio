using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenBots.Core.Gallery;
using OpenBots.Core.Gallery.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile;

namespace OpenBots.Utilities
{
    public class Project
    {
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Main { get; set; }

        public Project()
        {

        }

        public Project(string projectName)
        {
            ProjectID = Guid.NewGuid();
            ProjectName = projectName;
            Main = "Main.json";
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

        public static async Task<string> DownloadAndExtractProcessAsync(SearchResultPackage project, string targetDirectory)//Process process)
        {
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            var processNugetFilePath = Path.Combine(targetDirectory, project.Title  + ".nupkg");
            var processZipFilePath = Path.Combine(targetDirectory, project.Title + ".zip");

            // Check if Process (.nuget) file exists if Not Download it
            if (!File.Exists(processNugetFilePath))
            {
                var updater = new NugetPackageManger();
                var latestVersion = await updater.GetLatestVersionAsync(project.Id);
                await updater.DownloadAsync(project.Id, latestVersion, targetDirectory, project.Title, CancellationToken.None);
            }

            // Create .zip file
            File.Copy(processNugetFilePath, processZipFilePath, true);

            var extractToDirectoryPath = Path.ChangeExtension(processZipFilePath, null);

            // Extract Files/Folders from (.zip) file
            DecompressFile(processZipFilePath, extractToDirectoryPath);

            // Delete .zip File
            File.Delete(processZipFilePath);

            string configFilePath = Directory.GetFiles(extractToDirectoryPath, "project.config", SearchOption.AllDirectories).First();
            string mainFileName = JObject.Parse(File.ReadAllText(configFilePath))["Main"].ToString();

            // Return "Main" Script File Path of the Process
            return Directory.GetFiles(extractToDirectoryPath, mainFileName, SearchOption.AllDirectories).First();
        }

        private static void DecompressFile(string processZipFilePath, string targetDirectory)
        {
            // Create Target Directory If it doesn't exist
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);


            FileStream fs = File.OpenRead(processZipFilePath);
            ZipFile file = new ZipFile(fs);


            foreach (ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                {
                    // Ignore directories but create them in case they're empty
                    Directory.CreateDirectory(Path.Combine(targetDirectory, zipEntry.Name));
                    continue;
                }

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



            //// Extract Files/Folders from downloaded (.zip) file
            //FileStream fs = File.OpenRead(processZipFilePath);
            //ZipFile file = new ZipFile(fs);

            //foreach (ZipEntry zipEntry in file)
            //{
            //    if (!zipEntry.IsFile)
            //    {
            //        // Ignore directories
            //        continue;
            //    }

            //    string entryFileName = zipEntry.Name;

            //    // 4K is optimum
            //    byte[] buffer = new byte[4096];
            //    Stream zipStream = file.GetInputStream(zipEntry);

            //    // Manipulate the output filename here as desired.
            //    string fullZipToPath = Path.Combine(targetDirectory, entryFileName);
            //    string directoryName = Path.GetDirectoryName(fullZipToPath);

            //    if (directoryName.Length > 0)
            //        Directory.CreateDirectory(directoryName);

            //    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
            //    // of the file, but does not waste memory.
            //    // The "using" will close the stream even if an exception occurs.
            //    using (FileStream streamWriter = File.Create(fullZipToPath))
            //        StreamUtils.Copy(zipStream, streamWriter, buffer);
            //}

            //if (file != null)
            //{
            //    file.IsStreamOwner = true;
            //    file.Close();
            //}
        }
    }
}
