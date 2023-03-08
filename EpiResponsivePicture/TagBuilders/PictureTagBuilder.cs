using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Forte.EpiResponsivePicture.GeneratorProfiles;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Toolkit.Diagnostics;

namespace Forte.EpiResponsivePicture.TagBuilders;

public class PictureTagBuilder : IPictureTagBuilder
{
    private readonly TagBuilder element;
    private readonly IResizedUrlGenerator resizedUrlGenerator;
    private PictureProfile pictureProfile;
    private ContentReference pictureContentReference;
    private string pictureFallbackUrl;
    private ResizedPictureViewModel pictureViewModel;
    private FocalPoint focalPoint;
    private string pictureUrl;
    private string imgTagAltText;

    private PictureTagBuilder()
    {
        element = new TagBuilder("picture");
        resizedUrlGenerator = ServiceLocator.Current.GetInstance<IResizedUrlGenerator>();
    }

    public static IPictureTagBuilder Create() => new PictureTagBuilder();

    public IPictureTagBuilder WithProfile(PictureProfile profile)
    {
        pictureProfile = profile;
        return this;
    }

    public IPictureTagBuilder WithContentReference(ContentReference contentReference)
    {
        pictureContentReference = contentReference;
        return this;
    }

    public IPictureTagBuilder WithFallbackUrl(string fallbackUrl)
    {
        pictureFallbackUrl = fallbackUrl;
        return this;
    }

    public IPictureTagBuilder WithViewModel(ResizedPictureViewModel viewModel)
    {
        pictureViewModel = viewModel;
        return this;
    }

    public TagBuilder Build()
    {
        Guard.IsNotNull(pictureProfile, nameof(pictureProfile));

        pictureFallbackUrl ??= string.Empty;
        pictureViewModel ??= new ResizedPictureViewModel();

        SetPictureData();

        var sourceTagBuilder = SourceTagBuilder
            .Create()
            .WithImageUrl(pictureUrl)
            .WithFocalPoint(focalPoint)
            .WithProfile(pictureProfile);

        foreach (var pictureSource in pictureProfile.Sources)
        {
            var sourceTag = sourceTagBuilder.NewTag().WithSource(pictureSource).Build();

            element.InnerHtml.AppendHtml(sourceTag.RenderSelfClosingTag());
        }

        AddAdditionalAttributes();

        var imgTag = CreateImageElement();
        element.InnerHtml.AppendHtml(imgTag.RenderSelfClosingTag());

        return element;
    }

    private void SetPictureData()
    {
        var imageFound = ServiceLocator.Current.GetInstance<IContentLoader>().TryGet<IContentData>(
            pictureContentReference,
            new LoaderOptions { LanguageLoaderOption.FallbackWithMaster() }, out var content);

        if (imageFound)
        {
            if (content is IImage)
            {
                SetImageAltText((IImage) content);
            }
            if (content is IResponsiveImage)
            {
                SetFocalPoint((IResponsiveImage) content);
            }
        }

        pictureUrl = imageFound ? ResolveUrl() : pictureFallbackUrl;
    }

    private void SetImageAltText(IImage image)
    {
        imgTagAltText = image.Description ?? string.Empty;
    }

    private void SetFocalPoint(IResponsiveImage responsiveImage)
    {
        focalPoint = responsiveImage.FocalPoint ?? FocalPoint.Center;
    }

    private string ResolveUrl()
    {
        var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
        return urlResolver.GetUrl(pictureContentReference);
    }

    private void AddAdditionalAttributes()
    {
        foreach (var (key, value) in pictureViewModel.PictureElementAttributes)
        {
            element.Attributes.Add(key, value);
        }
    }

    private TagBuilder CreateImageElement()
    {
        var imgTagBuilder = new TagBuilder("img");

        var url = string.IsNullOrEmpty(pictureFallbackUrl)
            ? resizedUrlGenerator.GenerateUrl(pictureUrl, pictureProfile.DefaultWidth, null, null, focalPoint).ToString()
            : pictureFallbackUrl;

        imgTagBuilder.Attributes.Add("src", url);

        foreach (var imgElementAttribute in pictureViewModel.ImgElementAttributes)
        {
            imgTagBuilder.Attributes.Add(imgElementAttribute);
        }

        if (!string.IsNullOrEmpty(imgTagAltText))
        {
            imgTagBuilder.Attributes.TryAdd("alt", imgTagAltText);
        }

        return imgTagBuilder;
    }
}
