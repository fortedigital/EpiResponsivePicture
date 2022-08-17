using System;
using Microsoft.AspNetCore.Builder;

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
    public static IApplicationBuilder UseForteEpiResponsivePicture(this IApplicationBuilder app, Func<IApplicationBuilder, IApplicationBuilder> providerRegistration = null)
    {

        providerRegistration ??= SixLabors.ImageSharp.Web.DependencyInjection.ApplicationBuilderExtensions.UseImageSharp;
        
        providerRegistration(app);
        return app;
    }
}
