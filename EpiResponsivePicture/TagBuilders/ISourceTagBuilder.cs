using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Forte.EpiResponsivePicture.TagBuilders;

public interface ISourceTagBuilder
{
    ISourceTagBuilder WithImageUrl(string url);
    ISourceTagBuilder WithSource(PictureSource source);
    ISourceTagBuilder WithFocalPoint(FocalPoint point);
    ISourceTagBuilder WithResizedImageFormat(ResizedImageFormat format);
    TagBuilder Build();
    ISourceTagBuilder Clear();
}
