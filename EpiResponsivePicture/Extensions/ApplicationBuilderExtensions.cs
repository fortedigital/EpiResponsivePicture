using EPiServer.Core;
using Forte.EpiResponsivePicture.ResizedImage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using SixLabors.ImageSharp.Web.DependencyInjection;
// ReSharper disable UnusedMember.Global

namespace Forte.EpiResponsivePicture.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds ImageSharp. Fluent API
    /// </summary>
    /// <param name="app"></param>
    public static IApplicationBuilder UseForteEpiResponsivePicture(this IApplicationBuilder app)
    {
        RegisterImagePublishingEventHandler(app);
        
        app.UseImageSharp();
        return app;
    }
    
    private static void RegisterImagePublishingEventHandler(IApplicationBuilder app)
    {
        var imagePublishingEventHandler = app.ApplicationServices.GetService<ImagePublishingEventHandler>();
        if (imagePublishingEventHandler is null)
            ThrowHelper.ThrowInvalidOperationException(
                "Make sure to call services.AddForteEpiResponsivePicture() first");
        app.ApplicationServices.GetService<IContentEvents>()!.PublishingContent += imagePublishingEventHandler.CalculateDimensions;
    }
}
