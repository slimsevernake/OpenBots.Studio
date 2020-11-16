using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenBots.Core.Project;

namespace OpenBots.Core.Gallery
{
    public class NugetPackageManagerV2
    {
        //"https://api.nuget.org/v3/index.json"
        public static async Task<List<NuGetVersion>> GetPackageVersions(string packageId, string source)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3(source);
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(
                packageId,
                cache,
                logger,
                cancellationToken);

            foreach (NuGetVersion version in versions)
            {
                Console.WriteLine($"Found version {version}");
            }

            return versions.ToList();
        }

        public static async Task DownloadPackage(string packageId, string version, string source)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3(source);
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            NuGetVersion packageVersion = new NuGetVersion(version);
            using (MemoryStream packageStream = new MemoryStream())
            {
                await resource.CopyNupkgToStreamAsync(
                    packageId,
                    packageVersion,
                    packageStream,
                    cache,
                    logger,
                    cancellationToken);

                Console.WriteLine($"Downloaded package {packageId} {packageVersion}");

                using (PackageArchiveReader packageReader = new PackageArchiveReader(packageStream))
                {
                    NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

                    Console.WriteLine($"Tags: {nuspecReader.GetTags()}");
                    Console.WriteLine($"Description: {nuspecReader.GetDescription()}");
                }
            }
        }

        public static async Task<List<IPackageSearchMetadata>> SearchPackages(string packageKeyword, string source)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceRepository repository = Repository.Factory.GetCoreV3(source);
            PackageSearchResource resource = await repository.GetResourceAsync<PackageSearchResource>();
            SearchFilter searchFilter = new SearchFilter(includePrerelease: true);

            IEnumerable<IPackageSearchMetadata> results = await resource.SearchAsync(
                packageKeyword,
                searchFilter,
                skip: 0,
                take: 20,
                logger,
                cancellationToken);

            foreach (IPackageSearchMetadata result in results)
            {
                Console.WriteLine($"Found package {result.Identity.Id} {result.Identity.Version}");
            }

            return results.ToList();
        }

        public static async Task<List<IPackageSearchMetadata>> GetPackageMetadata(string packageId, string source)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3(source);
            PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();

            IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(
                packageId,
                includePrerelease: true,
                includeUnlisted: false,
                cache,
                logger,
                cancellationToken);

            foreach (IPackageSearchMetadata package in packages)
            {
                Console.WriteLine($"Version: {package.Identity.Version}");
                Console.WriteLine($"Listed: {package.IsListed}");
                Console.WriteLine($"Tags: {package.Tags}");
                Console.WriteLine($"Description: {package.Description}");
            }

            return packages.ToList();
        }

        public NuspecReader ReadPackage(string nugetFilePath)
        {
            using (FileStream inputStream = new FileStream(nugetFilePath, FileMode.Open))
            {
                using (PackageArchiveReader reader = new PackageArchiveReader(inputStream))
                {
                    NuspecReader nuspec = reader.NuspecReader;
                    Console.WriteLine($"ID: {nuspec.GetId()}");
                    Console.WriteLine($"Version: {nuspec.GetVersion()}");
                    Console.WriteLine($"Description: {nuspec.GetDescription()}");
                    Console.WriteLine($"Authors: {nuspec.GetAuthors()}");

                    Console.WriteLine("Dependencies:");
                    foreach (var dependencyGroup in nuspec.GetDependencyGroups())
                    {
                        Console.WriteLine($" - {dependencyGroup.TargetFramework.GetShortFolderName()}");
                        foreach (var dependency in dependencyGroup.Packages)
                        {
                            Console.WriteLine($"   > {dependency.Id} {dependency.VersionRange}");
                        }
                    }

                    Console.WriteLine("Files:");
                    foreach (var file in reader.GetFiles())
                    {
                        Console.WriteLine($" - {file}");
                    }

                    return nuspec;
                }
            }
        }

        [Obsolete]
        public static async Task InstallPackage(string packageId, string version, List<Dependency> projectDependenciesList)//, string source)
        {
            var packageVersion = NuGetVersion.Parse(version);
            var nuGetFramework = NuGetFramework.ParseFolder("net48");
            var settings = NuGet.Configuration.Settings.LoadDefaultSettings(root: null);

            //SourceRepository repository = Repository.Factory.GetCoreV3(source);

            var sourceRepositoryProvider = new SourceRepositoryProvider(settings, Repository.Provider.GetCoreV3());

            using (var cacheContext = new SourceCacheContext())
            {
                var repositories = sourceRepositoryProvider.GetRepositories();
                var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
                await GetPackageDependencies(
                    new PackageIdentity(packageId, packageVersion),
                    nuGetFramework, cacheContext, NullLogger.Instance, repositories, availablePackages);

                var resolverContext = new PackageResolverContext(
                    DependencyBehavior.Lowest,
                    new[] { packageId },
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<PackageReference>(),
                    Enumerable.Empty<PackageIdentity>(),
                    availablePackages,
                    sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                    NullLogger.Instance);

                var resolver = new PackageResolver();
                var packagesToInstall = resolver.Resolve(resolverContext, CancellationToken.None)
                    .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var packagePathResolver = new PackagePathResolver(Path.Combine(appDataPath, "OpenBots Inc", "packages"));//Path.GetFullPath("packages"));
                var packageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Defaultv3,
                    XmlDocFileSaveMode.None,
                    ClientPolicyContext.GetClientPolicy(settings, NullLogger.Instance),
                    NullLogger.Instance);
                    //new PackageSignatureVerifier(
                        //SignatureVerificationProviderFactory.GetSignatureVerificationProviders()),
                    //SignedPackageVerifierSettings.GetDefault());
                var frameworkReducer = new FrameworkReducer();

                foreach (var packageToInstall in packagesToInstall)
                {
                    PackageReaderBase packageReader;
                    var installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
                    if (installedPath == null)
                    {
                        var downloadResource = await packageToInstall.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                        var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                            packageToInstall,
                            new PackageDownloadContext(cacheContext),
                            SettingsUtility.GetGlobalPackagesFolder(settings),
                            NullLogger.Instance, CancellationToken.None);

                        await PackageExtractor.ExtractPackageAsync(
                            downloadResult.PackageSource,
                            downloadResult.PackageStream,
                            packagePathResolver,
                            packageExtractionContext,
                            CancellationToken.None);

                        packageReader = downloadResult.PackageReader;
                    }
                    else
                    {
                        packageReader = new PackageFolderReader(installedPath);
                    }

                    var libItems = packageReader.GetLibItems();
                    var nearest = frameworkReducer.GetNearest(nuGetFramework, libItems.Select(x => x.TargetFramework));
                    
                    Console.WriteLine(string.Join("\n", libItems
                        .Where(x => x.TargetFramework.Equals(nearest))
                        .SelectMany(x => x.Items)));           

                    string packageListAssemblyPath = libItems
                        .Where(x => x.TargetFramework.Equals(nearest))
                        .SelectMany(x => x.Items.Where(i => i.EndsWith(".dll"))).FirstOrDefault();

                    var existingPackage = projectDependenciesList.Where(x => x.PackageId == packageToInstall.Id).FirstOrDefault();
                    if (existingPackage != null)
                        projectDependenciesList.Remove(existingPackage);

                    projectDependenciesList.Add(new Dependency { 
                        PackageId = packageToInstall.Id, 
                        PackageVersion = packageToInstall.Version.ToString(), 
                        AssemblyPath = packageListAssemblyPath });

                    var frameworkItems = packageReader.GetFrameworkItems();
                    nearest = frameworkReducer.GetNearest(nuGetFramework, frameworkItems.Select(x => x.TargetFramework));
                    Console.WriteLine(string.Join("\n", frameworkItems
                        .Where(x => x.TargetFramework.Equals(nearest))
                        .SelectMany(x => x.Items)));
                }
            }            
        }

        public static async Task GetPackageDependencies(PackageIdentity package,
                NuGetFramework framework,
                SourceCacheContext cacheContext,
                ILogger logger,
                IEnumerable<SourceRepository> repositories,
                ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package)) return;

            foreach (var sourceRepository in repositories)
            {
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package, framework, cacheContext, logger, CancellationToken.None);

                if (dependencyInfo == null) continue;

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await GetPackageDependencies(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                        framework, cacheContext, logger, repositories, availablePackages);
                }
            }
        }

        public static List<Assembly> LoadProjectAssemblies(string configPath)
        {
            List<Assembly> assemblies = new List<Assembly>();
            var dependencies = JsonConvert.DeserializeObject<Project.Project>(File.ReadAllText(configPath)).Dependencies;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string packagePath = Path.Combine(appDataPath, "OpenBots Inc", "packages");

            //var provider = new
            //var pkgReader = new PackageReaderBase()

            foreach (var dependency in dependencies)
            {
                string dependencyPath = Path.Combine(packagePath, $"{dependency.PackageId}.{dependency.PackageVersion}", dependency.AssemblyPath);
                //var check = Directory.GetFiles(packagePath, pair.Key + ".dll", SearchOption.AllDirectories);
                //var engineAssemblyFilePath = Directory.GetFiles(packagePath, pair.Key + ".dll", SearchOption.AllDirectories).SingleOrDefault();
                if (File.Exists(dependencyPath))
                    assemblies.Add(Assembly.LoadFrom(dependencyPath));
                else
                    throw new Exception($"Assembly path for {dependencyPath} not found.");

            }
            return assemblies;
        }
    }
}

        