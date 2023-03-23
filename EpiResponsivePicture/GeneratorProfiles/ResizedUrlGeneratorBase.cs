using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using EPiServer;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.GeneratorProfiles;
using CustomQueryFunc = Func<int, PictureSource, ResizedImageFormat?, FocalPoint, (string Key, string Value)>;
using CustomQueryPredicate = Func<int, PictureSource, ResizedImageFormat?, FocalPoint, bool>;

public abstract class ResizedUrlGeneratorBase : IResizedUrlGenerator
{
    protected UrlBuilder Builder;

    private readonly Queue<(CustomQueryFunc Func, CustomQueryPredicate Predicate)> customQueryRegistrations = new();
    private readonly NameValueCollection customQueries = new();
    public UrlBuilder GenerateUrl(string imageUrl, int width, PictureSource pictureSource, ResizedImageFormat? format, FocalPoint focalPoint)
    {
        string imageExtension = Path.GetExtension(imageUrl);
        if (format is null or ResizedImageFormat.Preserve)
        {
            if (imageExtension is ".tif" or ".tiff")
            {
                format = ResizedImageFormat.Jpeg;
            }
        }
        Builder = new UrlBuilder(imageUrl);
            
        Builder.Add(WidthQuery(width));

        foreach (var (func, predicate) in customQueryRegistrations)
        {
            if(predicate(width, pictureSource, format, focalPoint))
                AddCustomQuery(func(width, pictureSource, format, focalPoint));
        }
            
        Builder.Add(customQueries);
            
        customQueries.Clear();

        return Builder;
    }
        
    protected abstract (string Key, string Value) WidthQuery(int width);

    protected void RegisterCustomQuery(CustomQueryFunc registration) =>
        customQueryRegistrations.Enqueue((registration, (_, _, _, _) => true));
    protected void RegisterCustomQuery(CustomQueryFunc registration, CustomQueryPredicate predicate) =>
        customQueryRegistrations.Enqueue((registration, predicate));
    private void AddCustomQuery((string Key, string Value) query)
    {
        customQueries.Add(query.Key, query.Value);
    }
}
