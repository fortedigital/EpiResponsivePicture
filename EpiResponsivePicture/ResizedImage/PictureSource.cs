using System.Collections.Immutable;

namespace Forte.EpiResponsivePicture.ResizedImage;

public class PictureSource
{
    public string MediaCondition { get; init; }
    public ImmutableArray<int> AllowedWidths { get; init; }
    public ImmutableArray<string> Sizes { get; init; }
    public ScaleMode Mode { get; init; }
    public AspectRatio TargetAspectRatio { get; init; }
    public int? Quality { get; init; }
}
