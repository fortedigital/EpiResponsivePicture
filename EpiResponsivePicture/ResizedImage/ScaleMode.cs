using Reinforced.Typings.Attributes;

namespace Forte.EpiResponsivePicture.ResizedImage;

// See https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Processing.ResizeMode.html for reference
[TsEnum]
public enum ScaleMode
{
    Default = 0,
    Crop,
    Pad,
    BoxPad,
    Max,
    Min,
    Stretch,
}
