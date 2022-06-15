using System.Collections.Immutable;

namespace Forte.EpiResponsivePicture.ResizedImage;

public class PictureProfile
{
    public int DefaultWidth { get; init; }
    public int MaxImageDimension { get; init; } = 3200; // Default max served by ImageResizer TODO check how it is in ImageSharp
    public ResizedImageFormat Format { get; init; } = ResizedImageFormat.Preserve;
    public ImmutableArray<PictureSource> Sources { get; init; } = ImmutableArray<PictureSource>.Empty;
}
