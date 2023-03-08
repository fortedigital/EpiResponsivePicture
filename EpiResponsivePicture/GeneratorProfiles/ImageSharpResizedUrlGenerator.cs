using System;
using System.Globalization;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.GeneratorProfiles;

public sealed class ImageSharpResizedUrlGenerator : ResizedUrlGeneratorBase
{
    private const string Height = "height";
    private const string Width = "width";
    private const string FocalPoint = "rxy";
    private const string Quality = "quality";
    private const string Format = "format";
    private const string Mode = "rmode";

    public ImageSharpResizedUrlGenerator()
    {
        RegisterCustomQuery(
            (width, pictureSource, _, _) => HeightQuery(width, pictureSource),
            (_, pictureSource, _, _) => pictureSource != null && pictureSource.TargetAspectRatio != AspectRatio.Original
        );
        RegisterCustomQuery(
            (_, pictureSource, _, _) => QualityQuery(pictureSource),
            (_, pictureSource, _, _) => pictureSource != null && pictureSource.Quality != PictureQuality.Default
        );
        RegisterCustomQuery(
            (_, _, _, focalPoint) => FocalPointQuery(focalPoint),
            (_, _, _, focalPoint) => focalPoint != null
            );
        RegisterCustomQuery(
            (_, pictureSource, _, _) => (Mode, $"{pictureSource.Mode.ToString()}"),
            (_, pictureSource, _, _) => pictureSource != null &&  pictureSource.Mode != ScaleMode.Default
            );
        RegisterCustomQuery(
            (_, _, pictureProfile, _) => FormatQuery(pictureProfile),
            (_, _, pictureProfile, _) => pictureProfile != null && pictureProfile.Format != ResizedImageFormat.Preserve
            );
    }

    protected override (string Key, string Value) WidthQuery(int width) => (Width, width.ToString());
    private (string Key, string Value) FocalPointQuery(FocalPoint focalPoint) =>
        (FocalPoint, $"{focalPoint.X:0.###},{focalPoint.Y:0.###}");
    private (string Key, string Value) HeightQuery(int width, PictureSource source) =>
        (Height, Math.Round(width / source.TargetAspectRatio.Ratio).ToString(CultureInfo.InvariantCulture));
    private (string Key, string Value) QualityQuery(PictureSource source) => (Quality, source.Quality.ToString());
    private (string Key, string Value) FormatQuery(PictureProfile profile) => (Format, profile.Format switch
    {
        ResizedImageFormat.Bmp => "BMP",
        ResizedImageFormat.Gif => "GIF",
        ResizedImageFormat.Png => "PNG",
        _ => "JPEG",
    });
}
