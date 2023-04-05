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
    protected readonly IResizedUrlGenerator ResizedUrlGenerator;
    protected readonly ISourceTagBuilderProvider SourceTagBuilderProvider;
    protected PictureProfile PictureProfile;
    protected ContentReference PictureContentReference;
    protected string PictureFallbackUrl;
    protected ResizedPictureViewModel PictureViewModel;
    protected FocalPoint FocalPoint;
    protected string PictureUrl;
    protected string ImgTagAltText;

    public PictureTagBuilder(IResizedUrlGenerator resizedUrlGenerator, ISourceTagBuilderProvider sourceTagBuilderProvider)
    {
        element = new TagBuilder("picture");
        ResizedUrlGenerator = resizedUrlGenerator;
        SourceTagBuilderProvider = sourceTagBuilderProvider;
    }

    public IPictureTagBuilder WithProfile(PictureProfile profile)
    {
        PictureProfile = profile;
        return this;
    }

    public IPictureTagBuilder WithContentReference(ContentReference contentReference)
    {
        PictureContentReference = contentReference;
        return this;
    }

    public IPictureTagBuilder WithFallbackUrl(string fallbackUrl)
    {
        PictureFallbackUrl = fallbackUrl;
        return this;
    }

    public IPictureTagBuilder WithViewModel(ResizedPictureViewModel viewModel)
    {
        PictureViewModel = viewModel;
        return this;
    }

    public TagBuilder Build()
    {
        Guard.IsNotNull(PictureProfile, nameof(PictureProfile));

        PictureFallbackUrl ??= string.Empty;
        PictureViewModel ??= new ResizedPictureViewModel();

        SetPictureData();

        var sourceTagBuilder = SourceTagBuilderProvider
            .Create()
            .WithImageUrl(PictureUrl)
            .WithFocalPoint(FocalPoint)
            .WithProfile(PictureProfile);

        foreach (var pictureSource in PictureProfile.Sources)
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
            PictureContentReference,
            new LoaderOptions { LanguageLoaderOption.FallbackWithMaster() }, out var content);

        if (imageFound)
        {
            if (content is IImage imageWithDescription)
            {
                SetImageAltText(imageWithDescription);
            }
            if (content is IResponsiveImage imageWithFocalPoint)
            {
                SetFocalPoint(imageWithFocalPoint);
            }
        }

        PictureUrl = imageFound ? ResolveUrl() : PictureFallbackUrl;
    }

    private void SetImageAltText(IImage image)
    {
        ImgTagAltText = image.Description ?? string.Empty;
    }

    private void SetFocalPoint(IResponsiveImage responsiveImage)
    {
        FocalPoint = responsiveImage.FocalPoint ?? FocalPoint.Center;
    }

    private string ResolveUrl()
    {
        var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
        return urlResolver.GetUrl(PictureContentReference);
    }

    private void AddAdditionalAttributes()
    {
        foreach (var (key, value) in PictureViewModel.PictureElementAttributes)
        {
            element.Attributes.Add(key, value);
        }
    }

    private TagBuilder CreateImageElement()
    {
        var imgTagBuilder = new TagBuilder("img");

        var url = string.IsNullOrEmpty(PictureFallbackUrl)
            ? ResizedUrlGenerator.GenerateUrl(PictureUrl, PictureProfile.DefaultWidth, null, PictureProfile, FocalPoint).ToString()
            : PictureFallbackUrl;

        imgTagBuilder.Attributes.Add("src", url);

        foreach (var imgElementAttribute in PictureViewModel.ImgElementAttributes)
        {
            imgTagBuilder.Attributes.Add(imgElementAttribute);
        }

        if (!string.IsNullOrEmpty(ImgTagAltText))
        {
            imgTagBuilder.Attributes.TryAdd("alt", ImgTagAltText);
        }

        return imgTagBuilder;
    }
}
