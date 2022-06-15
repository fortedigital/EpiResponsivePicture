using System;
using System.Globalization;
using Forte.EpiResponsivePicture.ResizedImage;

namespace Forte.EpiResponsivePicture.GeneratorProfiles;

public sealed class ImageSharpResizedUrlGenerator : ResizedUrlGeneratorBase
{
    private const string Height = "height";
    private const string Width = "width";
    private const string FocalPoint = "rxy";
    private const string Quality = "quailty";
    private const string Format = "format";

    private const string DefaultQuality = "80";
    
    public ImageSharpResizedUrlGenerator()
    {
        RegisterCustomQuery((_, _, _, focalPoint, _) => (FocalPoint, $"{focalPoint.X:0.###},{focalPoint.Y:0.###}"));
    }
    protected override (string Key, string Value) WidthQuery(int width) => (Width, width.ToString());

    protected override (string Key, string Value) HeightQuery(int width, PictureSource source,
        IImageWithWidthAndHeight imageDimensions) =>
        source.TargetAspectRatio != AspectRatio.Original
            ? (Height, Math.Round(width / source.TargetAspectRatio.Ratio).ToString(CultureInfo.InvariantCulture))
            : (Height,
                Math.Round(width / AspectRatio.Create(imageDimensions.Width, imageDimensions.Height).Ratio)
                    .ToString(CultureInfo.InvariantCulture));
    protected override (string Key, string Value) QualityQuery(PictureSource source) => 
        (Quality, source.Quality.HasValue ? source.Quality.Value.ToString() : DefaultQuality);

    protected override (string Key, string Value) FormatQuery(ResizedImageFormat format) => (Format, format switch
    {
        ResizedImageFormat.Bmp => "BMP",
        ResizedImageFormat.Gif => "GIF",
        ResizedImageFormat.Png => "PNG",
        _ => "JPEG",
    });
}
