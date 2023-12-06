using Baaijte.Optimizely.ImageSharp.Web.Providers;
using Forte.EpiResponsivePicture.Blob;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;
using System;
using System.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable once UnusedType.Global

namespace Forte.EpiResponsivePicture.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds ImageSharp. Fluent API
    /// </summary>
    /// <param name="app"></param>
    /// <param name="providerRegistration"></param>
    public static IApplicationBuilder UseForteEpiResponsivePicture(this IApplicationBuilder app,
        Func<IApplicationBuilder, IApplicationBuilder> providerRegistration = null)
    {
        return app.UseForteEpiResponsivePicture<BlobImageProvider>(providerRegistration);
    }

    /// <summary>
    /// Adds ImageSharp. Fluent API
    /// </summary>
    /// <param name="T">Specific type implementing IImageProvider</param>
    /// <param name="app"></param>
    /// <param name="providerRegistration"></param>
    public static IApplicationBuilder UseForteEpiResponsivePicture<T>(this IApplicationBuilder app,
        Func<IApplicationBuilder, IApplicationBuilder> providerRegistration = null) where T : IImageProvider
    {
        providerRegistration ??= SixLabors.ImageSharp.Web.DependencyInjection.ApplicationBuilderExtensions.UseImageSharp;
        providerRegistration(app);

        var imageProvider = app.ApplicationServices.GetServices<IImageProvider>()
            .FirstOrDefault(instance => instance is T);
        _ = imageProvider ?? throw new InvalidOperationException(
            $"{typeof(T).Name} is not found. Please make sure that it's added to ImageSharp service.");
        imageProvider.Match = app.ApplicationServices.GetService<IBlobSegmentsProvider>().IsMatch;

        return app;
    }
}
