using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.ResizedImage;

public interface IResponsiveImage
{
    FocalPoint FocalPoint { get; }
    int Width { get; set; }
    int Height { get; set; }
}
