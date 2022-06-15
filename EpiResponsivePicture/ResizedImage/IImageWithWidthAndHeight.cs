using System.Diagnostics;

namespace Forte.EpiResponsivePicture.ResizedImage;

public interface IImageWithWidthAndHeight
{
    int Width { get; set; }
    int Height { get; set; }
}
