using EPiServer.Core;

namespace Forte.EpiResponsivePicture.ResizedImage;

public interface IImage : IContentData, IImageWithWidthAndHeight
{
    string Description { get; }
}
