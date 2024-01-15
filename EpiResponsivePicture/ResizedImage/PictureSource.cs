using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace Forte.EpiResponsivePicture.ResizedImage;

[TsInterface]
public class PictureSource
{
    public string MediaCondition { get; init; }
    public IReadOnlyCollection<int> AllowedWidths { get; init; } = new List<int>();
    public IReadOnlyCollection<string> Sizes { get; init; } = new List<string>();
    public ScaleMode Mode { get; init; }
    public AspectRatio TargetAspectRatio { get; init; } = AspectRatio.Default;
    public PictureQuality Quality { get; init; } = PictureQuality.Default;
}
