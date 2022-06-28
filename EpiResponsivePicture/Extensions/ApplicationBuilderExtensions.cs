using System;
using EPiServer.Core;
using Forte.EpiResponsivePicture.ResizedImage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;

// ReSharper disable UnusedMember.Global

namespace Forte.EpiResponsivePicture.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds ImageSharp. Fluent API
    /// </summary>
    /// <param name="app"></param>
    /// <param name="providerRegistration"></param>
    public static IApplicationBuilder UseForteEpiResponsivePicture(this IApplicationBuilder app, Func<IApplicationBuilder, IApplicationBuilder> providerRegistration = null)
    {
        RegisterImagePublishingEventHandler(app);

        providerRegistration ??= SixLabors.ImageSharp.Web.DependencyInjection.ApplicationBuilderExtensions.UseImageSharp;
        
        providerRegistration(app);
        return app;
    }
    
    private static void RegisterImagePublishingEventHandler(IApplicationBuilder app)
    {
        var imagePublishingEventHandler = app.ApplicationServices.GetService<ImagePublishingEventHandler>();
        if (imagePublishingEventHandler is null)
            ThrowHelper.ThrowInvalidOperationException(
                "Make sure to call services.AddForteEpiResponsivePicture() first", 
                new ArgumentNullException(
                    nameof(ImagePublishingEventHandler), 
                    $"Could not resolve {nameof(ImagePublishingEventHandler)}"
                )
            );
        app.ApplicationServices.GetService<IContentEvents>()!.PublishingContent += imagePublishingEventHandler.CalculateDimensions;
    }
}
