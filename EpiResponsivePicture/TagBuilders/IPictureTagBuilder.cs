using EPiServer.Core;
using Forte.EpiResponsivePicture.ResizedImage;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Forte.EpiResponsivePicture.TagBuilders;

public interface IPictureTagBuilder
{
    IPictureTagBuilder WithProfile(PictureProfile profile);
    IPictureTagBuilder WithContentReference(ContentReference contentReference);
    IPictureTagBuilder WithFallbackUrl(string fallbackUrl);
    IPictureTagBuilder WithViewModel(ResizedPictureViewModel viewModel);
    TagBuilder Build();
}
