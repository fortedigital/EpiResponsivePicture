using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using EPiServer;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.GeneratorProfiles;
using CustomQueryFunc = Func<int, PictureSource, PictureProfile, FocalPoint, IImageWithWidthAndHeight, (string Key, string Value)>;

public abstract class ResizedUrlGeneratorBase : IResizedUrlGenerator
{
    protected UrlBuilder Builder;

    private readonly Queue<CustomQueryFunc> customQueryRegistrations = new();
    private readonly NameValueCollection customQueries = new();
    public UrlBuilder GenerateUrl(string imageUrl, int width, PictureSource pictureSource, PictureProfile pictureProfile, FocalPoint focalPoint, IImageWithWidthAndHeight imageDimensions)
    {
        Builder = new UrlBuilder(imageUrl);
            
        Builder.Add(WidthQuery(width));
        Builder.Add(HeightQuery(width, pictureSource, imageDimensions));
        Builder.Add(QualityQuery(pictureSource));
            
        if(pictureProfile.Format != ResizedImageFormat.Preserve)
            Builder.Add(FormatQuery(pictureProfile.Format));
            
        foreach (var customQueryRegistration in customQueryRegistrations)
        {
            AddCustomQuery(customQueryRegistration(width, pictureSource, pictureProfile, focalPoint, imageDimensions));
        }
            
        Builder.Add(customQueries);
            
        customQueries.Clear();

        return Builder;
    }
        
    protected abstract (string Key, string Value) WidthQuery(int width);
    protected abstract (string Key, string Value) HeightQuery(int width, PictureSource source, IImageWithWidthAndHeight imageDimensions);
    protected abstract (string Key, string Value) QualityQuery(PictureSource source);
    protected abstract (string Key, string Value) FormatQuery(ResizedImageFormat format);

    protected void RegisterCustomQuery(CustomQueryFunc registration) =>
        customQueryRegistrations.Enqueue(registration);
    private void AddCustomQuery((string Key, string Value) query)
    {
        customQueries.Add(query.Key, query.Value);
    }
}
