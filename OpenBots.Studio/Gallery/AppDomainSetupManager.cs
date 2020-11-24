using Autofac;
using OpenBots.Core.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace OpenBots.Gallery
{
    public class AppDomainSetupManager
    {
        public static AppDomain SetupAppDomain()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string packagePath = Path.Combine(appDataPath, "OpenBots Inc", "packages");
            string applicationBase = Path.GetDirectoryName(packagePath);
            AppDomainSetup setup = new AppDomainSetup
            {
               // ShadowCopyFiles = "true",
                //LoaderOptimization = LoaderOptimization.MultiDomainHost,
                ApplicationName = "OpenBots_Studio_AD",
                ApplicationBase = applicationBase,
                PrivateBinPath = applicationBase,//AppDomain.CurrentDomain.BaseDirectory,
                //PrivateBinPathProbe = AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            };

            Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
            var appDomain = AppDomain.CreateDomain("OpenBots_Studio_AD", evidence, setup);
            //appDomain.CreateInstanceFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Host.AssemblyLoader.dll"), "Host.AssemblyLoader");
            return appDomain;

        }

        public static void LoadDomain(List<string> assemblyPaths, List<Assembly> existingAssemblies)
        {
            List<string> retryPaths = new List<string>();
            assemblyPaths.Reverse();
            while (assemblyPaths.Count > 0)
            {
                foreach (var path in assemblyPaths)
                {
                    try
                    {
                        var name = AssemblyName.GetAssemblyName(path).ToString();

                        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                        var existingAssembly = assemblies.Where(x => x.GetName().ToString() == name).FirstOrDefault();
                        if (existingAssembly == null)
                        {
                            var assembly = Assembly.Load(File.ReadAllBytes(path));
                            existingAssemblies.Add(assembly);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        retryPaths.Add(path);
                    }
                }
                assemblyPaths.Clear();
                assemblyPaths.AddRange(retryPaths);
                assemblyPaths.Reverse();
                retryPaths.Clear();
            }
        }

        public static ContainerBuilder LoadBuilder(List<Assembly> assemblies)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(assemblies.ToArray())
                                                   .Where(t => t.IsAssignableTo<ScriptCommand>())
                                                   .Named<ScriptCommand>(t => t.Name)
                                                   .AsImplementedInterfaces();
            return builder;
        }
    }
}
