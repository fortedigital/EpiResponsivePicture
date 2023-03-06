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
    private PictureProfile pictureProfile;

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
    
    public ISourceTagBuilder WithProfile(PictureProfile profile)
    {
        pictureProfile = profile;
        return this;
    }

    public ISourceTagBuilder WithFocalPoint(FocalPoint point)
    {
        focalPoint = point;
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

    public ISourceTagBuilder NewTag()
    {
        element = new TagBuilder("source");
        return this;
    }

    private void AddSizeAttributes(IEnumerable<string> sourceSets)
    {
        if(!string.IsNullOrEmpty(pictureSource.MediaCondition))
            element.Attributes.Add("media", $"{Remove(pictureSource.MediaCondition)}");
        element.Attributes.Add("srcset", string.Join(", ", sourceSets));
        element.Attributes.Add("sizes", string.Join(", ", pictureSource.Sizes));
    }
    private static string Remove(string mediaCondition) => $"({mediaCondition.Trim('(', ')', ' ')})"; 
    private IEnumerable<string> GetSourceSets() => pictureSource.AllowedWidths.Select(BuildWidth);  

    private string BuildWidth(int width)
    {
        var url = resizedUrlGenerator.GenerateUrl(imageUrl, width, pictureSource, pictureProfile, focalPoint);

        return $"{url} {width}w";
    }

}
