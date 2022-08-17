using EPiServer.Core;

namespace Forte.EpiResponsivePicture.ResizedImage;

public interface IImage : IContentData
{
    string Description { get; }
}
