using Autofac;
using OpenBots.Core.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;

namespace OpenBots.Nuget
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

        public static ContainerBuilder LoadBuilder(List<string> assemblyPaths)
        {
            List<Assembly> existingAssemblies = new List<Assembly>();
            foreach (var path in assemblyPaths)
            {
                try
                {
                    var assemblyinfo = AssemblyName.GetAssemblyName(path);
                    var name = assemblyinfo.Name;
                    var version = assemblyinfo.Version.ToString();

                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var existingAssembly = assemblies.Where(x => x.GetName().Name == name && 
                                                                 x.GetName().Version.ToString() == version)
                                                     .FirstOrDefault();

                    if (existingAssembly == null)
                    {
                        var assembly = Assembly.LoadFrom(path);
                        existingAssemblies.Add(assembly);
                    }
                    else
                        existingAssemblies.Add(existingAssembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(existingAssemblies.ToArray())
                                                   .Where(t => t.IsAssignableTo<ScriptCommand>())
                                                   .Named<ScriptCommand>(t => t.Name)
                                                   .AsImplementedInterfaces();
            return builder;
        }
    }
}
