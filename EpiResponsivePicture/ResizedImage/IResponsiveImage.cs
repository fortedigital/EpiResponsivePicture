using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.ResizedImage;

public interface IResponsiveImage : IImageWithWidthAndHeight
{
    FocalPoint FocalPoint { get; }
}
