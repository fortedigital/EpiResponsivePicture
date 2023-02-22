using System;
using System.Linq;
using Baaijte.Optimizely.ImageSharp.Web.Providers;
using Forte.EpiResponsivePicture.Blob;
using Microsoft.AspNetCore.Builder;
using SixLabors.ImageSharp.Web.Providers;
using Microsoft.Extensions.DependencyInjection;

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
        providerRegistration ??= SixLabors.ImageSharp.Web.DependencyInjection.ApplicationBuilderExtensions.UseImageSharp;
        providerRegistration(app);

        var imageProvider = app.ApplicationServices.GetServices<IImageProvider>()
            .FirstOrDefault(instance => instance.GetType() == typeof(BlobImageProvider));
        _ = imageProvider ?? throw new InvalidOperationException(
            "BlobImageProvider is not found. Please make sure that it's added to ImageSharp service.");
        imageProvider.Match = app.ApplicationServices.GetService<IBlobSegmentsProvider>().IsMatch;

        return app;
    }
}
