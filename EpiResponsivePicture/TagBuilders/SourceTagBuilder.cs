using System.Collections.Generic;
using System.Linq;
using EPiServer.ServiceLocation;
using Forte.EpiResponsivePicture.GeneratorProfiles;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Toolkit.Diagnostics;

namespace Forte.EpiResponsivePicture.TagBuilders;

public class SourceTagBuilder : ISourceTagBuilder
{
    private TagBuilder element;
    private string imageUrl;
    private PictureSource pictureSource;
    private IResizedUrlGenerator resizedUrlGenerator;
    private FocalPoint focalPoint;
    private ResizedImageFormat resizedImageFormat;
        
    private SourceTagBuilder()
    {
        element = new TagBuilder("source");
        resizedUrlGenerator = ServiceLocator.Current.GetInstance<IResizedUrlGenerator>();
    }

    public static ISourceTagBuilder Create() => new SourceTagBuilder();

    public ISourceTagBuilder WithImageUrl(string url)
    {
        imageUrl = url;
        return this;
    }

    public ISourceTagBuilder WithSource(PictureSource source)
    {
        pictureSource = source;
        return this;
    }

    public ISourceTagBuilder WithFocalPoint(FocalPoint point)
    {
        focalPoint = point;
        return this;
    }

    public ISourceTagBuilder WithResizedImageFormat(ResizedImageFormat format)
    {
        resizedImageFormat = format;
        return this;
    }
        
    public TagBuilder Build()
    {
        Guard.IsNotNull(pictureSource, nameof(pictureSource));
        Guard.IsNotNullOrEmpty(imageUrl, nameof(imageUrl));

        var sourceSets = GetSourceSets();
            
        AddSizeAttributes(sourceSets);
            
        return element;
    }

    public ISourceTagBuilder Clear()
    {
        element = new TagBuilder("source");
        return this;
    }

    private void AddSizeAttributes(IEnumerable<string> sourceSets)
    {
        element.Attributes.Add("media", $"{pictureSource.MediaCondition}");
        element.Attributes.Add("srcset", string.Join(", ", sourceSets));
        element.Attributes.Add("sizes", string.Join(", ", pictureSource.Sizes));
    }
        
    private IEnumerable<string> GetSourceSets() => pictureSource.AllowedWidths.Select(BuildSize);  

    private string BuildSize(int width)
    {
        var url = resizedUrlGenerator.GenerateUrl(imageUrl, width, pictureSource, focalPoint, resizedImageFormat);

        return $"{url} {width}w";
    }

}
