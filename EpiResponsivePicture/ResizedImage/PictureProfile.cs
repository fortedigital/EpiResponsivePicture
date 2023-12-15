using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace Forte.EpiResponsivePicture.ResizedImage;

[TsInterface(AutoExportMethods = false, AutoExportProperties = true)]
public class PictureProfile
{
    public int DefaultWidth { get; init; }
    public ResizedImageFormat Format { get; init; } = ResizedImageFormat.Preserve;
    public IReadOnlyCollection<PictureSource> Sources { get; init; } = new List<PictureSource>();
    public PictureProfile CopyWithNewFormat(ResizedImageFormat format)
    {
        return new PictureProfile
        {
            DefaultWidth = this.DefaultWidth,
            Format = format,
            Sources = this.Sources
        };
    }
}
