using EPiServer;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.GeneratorProfiles;

public interface IResizedUrlGenerator
{
    public UrlBuilder GenerateUrl(string imageUrl, int width, PictureSource pictureSource, PictureProfile pictureProfile, FocalPoint focalPoint, IImageWithWidthAndHeight imageDimensions);
}
