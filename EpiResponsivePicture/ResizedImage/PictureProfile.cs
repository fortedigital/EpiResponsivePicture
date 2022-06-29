using System.Collections.Immutable;

namespace Forte.EpiResponsivePicture.ResizedImage;

public class PictureProfile
{
    public int DefaultWidth { get; init; }
    public int MaxImageDimension { get; init; } = 3200;
    public ResizedImageFormat Format { get; init; } = ResizedImageFormat.Preserve;
    public ImmutableArray<PictureSource> Sources { get; init; } = ImmutableArray<PictureSource>.Empty;
}
