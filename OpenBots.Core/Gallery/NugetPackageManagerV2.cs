using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OpenBots.Core.Gallery
{
    public class NugetPackageManagerV2
    {
        //"https://api.nuget.org/v3/index.json"
        public static async void GetPackageVersions(string packageId, string source)
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
        }

        public static async void DownloadPackage(string packageId, string version, string source)
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

        public static async void SearchPackages(string packageKeyword, string source)
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
        }

        public void ReadPackage(string nugetFilePath)
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
                }
                    
            }
               
        }
    }
}
