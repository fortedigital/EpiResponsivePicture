using System.Collections.Immutable;

namespace Forte.EpiResponsivePicture.ResizedImage;

public class PictureSource
{
    public string MediaCondition { get; init; }
    public ImmutableArray<int> AllowedWidths { get; init; } = ImmutableArray<int>.Empty;
    public ImmutableArray<string> Sizes { get; init; } = ImmutableArray<string>.Empty;
    public ScaleMode Mode { get; init; }
    public AspectRatio TargetAspectRatio { get; init; } = AspectRatio.Default;
    public int? Quality { get; init; }
}
