using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using EPiServer;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.GeneratorProfiles;
using CustomQueryFunc = Func<int, PictureSource, PictureProfile, FocalPoint, (string Key, string Value)>;
using CustomQueryPredicate = Func<int, PictureSource, PictureProfile, FocalPoint, bool>;

public abstract class ResizedUrlGeneratorBase : IResizedUrlGenerator
{
    private readonly Queue<(CustomQueryFunc Func, CustomQueryPredicate Predicate)> customQueryRegistrations = new();
    protected IReadOnlyCollection<string> ExtensionsToReplace { get; init; } = new List<string> { ".tif", ".tiff" };
    public UrlBuilder GenerateUrl(string imageUrl, int width, PictureSource pictureSource, PictureProfile pictureProfile, FocalPoint focalPoint)
    {
        var imageExtension = Path.GetExtension(imageUrl);
        var copiedPictureProfile =
            pictureProfile.Format is ResizedImageFormat.Preserve && ExtensionsToReplace.Contains(imageExtension)
                ? pictureProfile.CopyWithNewFormat(ResizedImageFormat.Jpeg)
                : pictureProfile;
        var builder = new UrlBuilder(imageUrl);
        var customQueries = new NameValueCollection();

        builder.Add(WidthQuery(width));

        foreach (var (func, predicate) in customQueryRegistrations)
        {
            if (predicate(width, pictureSource, copiedPictureProfile, focalPoint))
            {
                AddCustomQuery(func(width, pictureSource, copiedPictureProfile, focalPoint));
            }
        }

        if (customQueries.Count > 0)
        {
            builder.Add(customQueries);
        }

        return builder;

        void AddCustomQuery((string Key, string Value) query)
        {
            customQueries.Add(query.Key, query.Value);
        }
    }

    protected abstract (string Key, string Value) WidthQuery(int width);

    protected void RegisterCustomQuery(CustomQueryFunc registration) =>
        customQueryRegistrations.Enqueue((registration, (_, _, _, _) => true));
    protected void RegisterCustomQuery(CustomQueryFunc registration, CustomQueryPredicate predicate) =>
        customQueryRegistrations.Enqueue((registration, predicate));
}
