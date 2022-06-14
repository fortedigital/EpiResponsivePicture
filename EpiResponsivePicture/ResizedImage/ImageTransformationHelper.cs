using System.IO;
using System.Text.Encodings.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Forte.EpiResponsivePicture.TagBuilders;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Forte.EpiResponsivePicture.ResizedImage;

public static class ImageTransformationHelper
{
    public static UrlBuilder ResizedImageUrl(this IHtmlHelper helper, ContentReference image, int width,
        ResizedImageFormat format = ResizedImageFormat.Preserve)
    {
        var baseUrl = ResolveImageUrl(image);
                
        var urlBuilder = new UrlBuilder(baseUrl)
            .Add("width", width.ToString());
                
        if(format != ResizedImageFormat.Preserve) 
            urlBuilder.Add("format", format.ToString().ToUpperInvariant());

        return urlBuilder;
    }

    public static HtmlString ResizedPicture(this IHtmlHelper helper,
        ContentReference image,
        PictureProfile profile,
        string fallbackUrl = null,
        ResizedPictureViewModel pictureModel = null)
    {

        var pictureTag = PictureTagBuilder
            .Create()
            .WithContentReference(image)
            .WithProfile(profile)
            .WithFallbackUrl(fallbackUrl)
            .WithViewModel(pictureModel)
            .Build();

        using var writer = new StringWriter();
            
        pictureTag.WriteTo(writer, HtmlEncoder.Default);

        return new HtmlString(writer.ToString());
    }
        
    private static string ResolveImageUrl(ContentReference image)
    {
        var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
        return urlResolver.GetUrl(image);
    }
}
