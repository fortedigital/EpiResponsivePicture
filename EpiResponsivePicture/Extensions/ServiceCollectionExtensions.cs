using Azure.Storage.Blobs.Models;
using Baaijte.Optimizely.ImageSharp.Web.Caching;
using Baaijte.Optimizely.ImageSharp.Web.Providers;
using EPiServer.Shell.Modules;
using Forte.EpiResponsivePicture.Blob;
using Forte.EpiResponsivePicture.Configuration;
using Forte.EpiResponsivePicture.GeneratorProfiles;
using Forte.EpiResponsivePicture.Middlewares;
using Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility.SqlProvider;
using Forte.EpiResponsivePicture.TagBuilders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SixLabors.ImageSharp.Web.Caching.Azure;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;
using System;
using System.Linq;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Forte.EpiResponsivePicture.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register services for FocalPoint, resizing with local caching. Fluent API
    /// </summary>
    /// <param name="services">Services</param>
    /// <param name="options"></param>
    /// <returns>services</returns>
    public static IServiceCollection AddForteEpiResponsivePicture(this IServiceCollection services,
        EpiResponsivePicturesOptions options = null)
    {
        services.AddImageSharp()
            .ClearProviders()
            .AddProvider<BlobImageProvider>()
            .AddProvider<PhysicalFileSystemProvider>()
            .SetCache<BlobImageCache>();

        ConfigureModule(services, options);

        return services;
    }

    /// <summary>
    /// Register services for FocalPoint, resizing with Azure Blob Storage caching. Fluent API
    /// </summary>
    /// <param name="services">Services</param>
    /// <param name="azureStorageOptions">Azure blob storage options</param>
    /// <param name="options"></param>
    /// <returns>services</returns>
    public static IServiceCollection AddForteEpiResponsivePicture(this IServiceCollection services,
        Action<AzureBlobStorageCacheOptions> azureStorageOptions,
        EpiResponsivePicturesOptions options = null)
    {
        azureStorageOptions += ValidateOptions;
        azureStorageOptions += CreateContainerIfNotExists;
        services.AddImageSharp()
            .Configure(azureStorageOptions)
            .ClearProviders()
            .AddProvider<BlobImageProvider>()
            .SetCache<AzureBlobStorageCache>();

        ConfigureModule(services, options);

        return services;
    }

    /// <summary>
    /// Creates container with provided name, when it does not exist
    /// </summary>
    /// <param name="options">Azure blob storage cache options</param>
    private static void CreateContainerIfNotExists(AzureBlobStorageCacheOptions options)
    {
        AzureBlobStorageCache.CreateIfNotExists(options, PublicAccessType.None);
    }

    /// <summary>
    /// Checks if necessary options were provided, otherwise throws exception
    /// </summary>
    /// <param name="options">Azure blob storage cache options</param>
    /// <exception cref="Exception">Thrown when any of required options is missing</exception>
    private static void ValidateOptions(AzureBlobStorageCacheOptions options)
    {
        if (string.IsNullOrEmpty(options.ConnectionString))
        {
            throw new ArgumentException("Need to provide connection string!");
        }
        if (string.IsNullOrEmpty(options.ContainerName))
        {
            throw new ArgumentException("Need to provide container name!");
        }
    }

    private static void ConfigureModule(IServiceCollection services, EpiResponsivePicturesOptions options = null)
    {
        services.TryAddTransient<IResizedUrlGenerator, ImageSharpResizedUrlGenerator>();

        services.Configure<ProtectedModuleOptions>(o =>
        {
            if (!o.Items.Any(x => x.Name.Equals(Constants.ModuleName)))
            {
                o.Items.Add(
                    new ModuleDetails
                    {
                        Name = Constants.ModuleName,
                    }
                );
            }
        });

        services
            .AddOptions<EpiResponsivePicturesOptions>()
            .Configure(o =>
            {
                o.ImageResizerCompatibilityEnabled = options?.ImageResizerCompatibilityEnabled ?? false;
                o.AdditionalSegments = options?.AdditionalSegments;
                o.MaxPictureSize = options?.MaxPictureSize;
            });

        services.AddSingleton<IBlobSegmentsProvider, BlobCustomSegmentsProvider>();
        services.AddTransient<IImageResizerFocalPointConversionSqlProvider, ImageResizerFocalPointConversionSqlProvider>();
        services.AddTransient<IPictureTagBuilderProvider, PictureTagBuilderProvider>();
        services.AddTransient<ISourceTagBuilderProvider, SourceTagBuilderProvider>();

        if (options?.MaxPictureSize is not null)
        {
            services.AddScoped<ImageSizeLimitMiddleware>();
        }
    }
}
