using System;
using System.Linq;
using System.Text;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Forte.EpiResponsivePicture.ResizedImage.Background;

public static class ResizeBackgroundHelper
{
    private const int MaxResizedImageWidth = 3200;

    public static string ResizeBackground(this HtmlHelper helper, ContentReference image,
        BackgroundPictureProfile profile)
    {
        if (image == null)
            return "error-no-image";

        var className = "_" + Guid.NewGuid().ToString("N");
        var styles = GenerateMediaQueryStyles(helper, profile, image, className);
        helper.Display(styles);
        return className;
    }

    private static string GenerateMediaQueryStyles(HtmlHelper helper, BackgroundPictureProfile profile,
        ContentReference image, string className)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("<style>");

        foreach (var allowedSize in profile.AllowedSizes.Reverse())
        {
            var mediaQuery = helper.Encode(allowedSize.MediaCondition);

            RenderNonRetinaCss(helper, image, mediaQuery, stringBuilder, className, allowedSize, profile.Format);
            RenderRetinaCss(helper, image, mediaQuery, stringBuilder, className, allowedSize, profile.Format);
        }

        stringBuilder.Append("</style>");
        return stringBuilder.ToString();
    }

    private static void RenderRetinaCss(HtmlHelper helper, ContentReference image, string mediaQuery,
        StringBuilder stringBuilder, string className, PictureSize allowedSize, ResizedImageFormat format)
    {
        var mediaQuerySelector = string.IsNullOrEmpty(mediaQuery)
            ? ""
            : $"and ({mediaQuery})";
        stringBuilder.Append($@"@media (-webkit-min-device-pixel-ratio: 2) {mediaQuerySelector},
                                                   (min--moz-device-pixel-ratio: 2)    {mediaQuerySelector},
                                                   (-o-min-device-pixel-ratio: 2/1)    {mediaQuerySelector},
                                                   (   min-device-pixel-ratio: 2)      {mediaQuerySelector} {{");

        RenderBackgroundCssClass(helper, image, stringBuilder, className,
            Math.Min(allowedSize.ImageWidth * 2, MaxResizedImageWidth), format);
        stringBuilder.Append("}");
    }

    private static void RenderNonRetinaCss(HtmlHelper helper, ContentReference image, string mediaQuery,
        StringBuilder stringBuilder, string className, PictureSize allowedSize, ResizedImageFormat format)
    {
        if (!string.IsNullOrEmpty(mediaQuery))
            stringBuilder.Append($"@media ({mediaQuery}) {{");

        RenderBackgroundCssClass(helper, image, stringBuilder, className, Math.Min(allowedSize.ImageWidth,
            MaxResizedImageWidth), format);

        if (!string.IsNullOrEmpty(mediaQuery))
            stringBuilder.Append("}"); // media query close
    }

    private static void RenderBackgroundCssClass(HtmlHelper helper, ContentReference image,
        StringBuilder stringBuilder,
        string className, int imageWidth, ResizedImageFormat format)
    {
        stringBuilder.Append($".{className} {{ ");

        stringBuilder.Append(
            $"background-image: url('{helper.ResizedImageUrl(image, imageWidth, format)}');");

        stringBuilder.Append("}"); // css class close
    }
}