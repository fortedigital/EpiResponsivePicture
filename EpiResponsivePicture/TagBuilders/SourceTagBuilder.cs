using System.Collections.Generic;
using System.Linq;
using Forte.EpiResponsivePicture.GeneratorProfiles;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Toolkit.Diagnostics;

namespace Forte.EpiResponsivePicture.TagBuilders;

public class SourceTagBuilder : ISourceTagBuilder
{
    private TagBuilder element;
    protected string ImageUrl;
    protected PictureSource PictureSource;
    protected readonly IResizedUrlGenerator ResizedUrlGenerator;
    protected FocalPoint FocalPoint;
    protected PictureProfile PictureProfile;

    public SourceTagBuilder(IResizedUrlGenerator resizedUrlGenerator)
    {
        element = new TagBuilder("source");
        ResizedUrlGenerator = resizedUrlGenerator;
    }

    public ISourceTagBuilder WithImageUrl(string url)
    {
        ImageUrl = url;
        return this;
    }

    public ISourceTagBuilder WithSource(PictureSource source)
    {
        PictureSource = source;
        return this;
    }

    public ISourceTagBuilder WithProfile(PictureProfile profile)
    {
        PictureProfile = profile;
        return this;
    }

    public ISourceTagBuilder WithFocalPoint(FocalPoint point)
    {
        FocalPoint = point;
        return this;
    }

    public TagBuilder Build()
    {
        Guard.IsNotNull(PictureSource, nameof(PictureSource));
        Guard.IsNotNullOrEmpty(ImageUrl, nameof(ImageUrl));

        var sourceSets = GetSourceSets();

        AddAttributes(sourceSets);

        return element;
    }

    public ISourceTagBuilder NewTag()
    {
        element = new TagBuilder("source");
        return this;
    }

    private void AddAttributes(IEnumerable<string> sourceSets)
    {
        if(!string.IsNullOrEmpty(PictureSource.MediaCondition))
            element.Attributes.Add("media", $"{EnsureBrackets(PictureSource.MediaCondition)}");
        element.Attributes.Add("srcset", string.Join(", ", sourceSets));
        element.Attributes.Add("sizes", string.Join(", ", PictureSource.Sizes));
    }
    private static string EnsureBrackets(string mediaCondition) => $"({mediaCondition.Trim('(', ')', ' ')})";
    protected virtual IEnumerable<string> GetSourceSets() => PictureSource.AllowedWidths.Select(BuildWidth);

    private string BuildWidth(int width)
    {
        var url = ResizedUrlGenerator.GenerateUrl(ImageUrl, width, PictureSource, PictureProfile, FocalPoint);

        return $"{url} {width}w";
    }
}
