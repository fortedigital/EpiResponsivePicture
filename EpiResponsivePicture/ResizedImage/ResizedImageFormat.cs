using System;
using Reinforced.Typings.Attributes;

namespace Forte.EpiResponsivePicture.ResizedImage;

[TsEnum]
public enum ResizedImageFormat
{
    /// <summary>
    /// Keeps the original format of the image being resized
    /// </summary>
    Preserve = 0,
    [Obsolete($"Use {nameof(Jpeg)} instead")]
    Jpg = Jpeg,
    Jpeg = 1,
    Png = 2,
    Gif = 3,
    Bmp = 4,
}
