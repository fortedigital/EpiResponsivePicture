using System;
using System.Linq;
using Baaijte.Optimizely.ImageSharp.Web.Caching;
using Baaijte.Optimizely.ImageSharp.Web.Providers;
using EPiServer.Shell.Modules;
using Forte.EpiResponsivePicture.Configuration;
using Forte.EpiResponsivePicture.GeneratorProfiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SixLabors.ImageSharp.Web.Caching.Azure;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Forte.EpiResponsivePicture.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register services for FocalPoint, resizing with local caching. Fluent API
    /// </summary>
    /// <param name="services">Services</param>
    /// <returns>services</returns>
    public static IServiceCollection AddForteEpiResponsivePicture(this IServiceCollection services)
    {

        services.AddImageSharp()
            .ClearProviders()
            .AddProvider<BlobImageProvider>()
            .AddProvider<PhysicalFileSystemProvider>()
            .SetCache<BlobImageCache>();
            
        ConfigureModule(services);
                   
        return services;
    }

    /// <summary>
    /// Register services for FocalPoint, resizing with Azure Blob Storage caching. Fluent API
    /// </summary>
    /// <param name="services">Services</param>
    /// <param name="azureStorageOptions">Azure blob storage options</param>
    /// <returns>services</returns>
    public static IServiceCollection AddForteEpiResponsivePicture(this IServiceCollection services,
        Action<AzureBlobStorageCacheOptions> azureStorageOptions)
    {
        services.AddImageSharp()
            .Configure(azureStorageOptions)
            .ClearProviders()
            .AddProvider<BlobImageProvider>()
            .SetCache<AzureBlobStorageCache>();
            
        ConfigureModule(services);

        return services;
    }

    private static void ConfigureModule(IServiceCollection services)
    {
        services.TryAddTransient<IResizedUrlGenerator, ImageSharpResizedUrlGenerator>();
            
        services.Configure<ProtectedModuleOptions>(options =>
        {
            if (!options.Items.Any(x => x.Name.Equals(Constants.ModuleName)))
            {
                options.Items.Add(
                    new ModuleDetails
                    {
                        Name = Constants.ModuleName,
                    }
                );
            }
        });
    }
}
