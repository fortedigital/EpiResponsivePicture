using Microsoft.AspNetCore.Builder;
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
        app.UseImageSharp();
        return app;
    }
}
