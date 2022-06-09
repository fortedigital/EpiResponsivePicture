using System;
using System.Globalization;
using Forte.EpiResponsivePicture.ResizedImage;

namespace Forte.EpiResponsivePicture.GeneratorProfiles
{
    public sealed class ImageSharpResizedUrlGenerator : ResizedUrlGeneratorBase
    {
        public ImageSharpResizedUrlGenerator()
        {
            RegisterCustomQuery((_, _, focalPoint) => ("rxy", $"{focalPoint.X:0.###},{focalPoint.Y:0.###}"));
        }
        protected override (string Key, string Value) WidthQuery(int width) => ("width", width.ToString());

        protected override (string Key, string Value) HeightQuery(int width, PictureSource source) => ("height",
            Math.Round(width / source.TargetAspectRatio.Ratio).ToString(CultureInfo.InvariantCulture));

        protected override (string Key, string Value) QualityQuery(PictureSource source) => ("quality",
            source.Quality.HasValue ? source.Quality.Value.ToString() : "90");

        protected override (string Key, string Value) FormatQuery(ResizedImageFormat format) => ("format", format switch
        {
            ResizedImageFormat.Bmp => "BMP",
            ResizedImageFormat.Gif => "GIF",
            ResizedImageFormat.Png => "PNG",
            _ => "JPEG",
        });
    }
}