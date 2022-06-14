using System;

namespace Forte.EpiResponsivePicture.ResizedImage;

public enum ResizedImageFormat
{
    /// <summary>
    /// Keeps the original format of the image being resized
    /// </summary>
    Preserve = 0,
    [Obsolete($"Use {nameof(Jpeg)} instead")]
    Jpg = Jpeg,
    Jpeg = 1,
    Png,
    Gif,
    Bmp,
}
