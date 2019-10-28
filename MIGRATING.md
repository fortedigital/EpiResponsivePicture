# Migration guide from Forte.EpiCommonUtils

To successfully migrate from previous version of Responsive images (contained in Forte.EpiCommonUtils package):
1. Update package Forte.EpiCommonUtils to version with responsive pictures extracted (min. VERSION_MISSING)
1. Install this NuGet package (Forte.EpiResponsivePicture)
1. Update namespace usings: 
    - Forte.EpiCommonUtils.Infrastructure.Model.ImageBase -> Forte.EpiResponsivePicture.ResizedImage.ImageBase
    - Forte.EpiCommonUtils.ResizedImage.PictureProfile -> Forte.EpiResponsivePicture.ResizedImage.PictureProfile
    - Forte.EpiCommonUtils.ResizedImage.ScaleMode -> Forte.EpiResponsivePicture.ResizedImage.ScaleMode
    - Forte.EpiCommonUtils.ResizedImage.ImageTransformationHelper -> Forte.EpiResponsivePicture.ResizedImage.ImageTransformationHelper
1. Migrate all picture profiles. Instead of properties `SrcSetSizes`, `SrcSetWidths`, `Mode` and `Quality`, add property `Sources` with single `PictureSource` item, containing following values of properties:
    - Sizes - value of current SrcSetSizes,
    - AllowedWidths - value of current SetWidths,
    - Quality and Mode - value of picture profile mirror properties.
1. As new ImageBase have additional `Width` and `Height` properties, that gets calculated on image publishing automatically, it is necessary to do this for all existing images. The easiest way is probably to republish all the images and let event handler do this for us, for example using following job:

```cs
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.Security;

[ScheduledPlugIn(DisplayName = "Republish all images",
        Description =
            "This job removes iterates through all images and republish all to calculate dimensions for responsive images")]
    public class RepublishAllImages : ScheduledJobBase
    {
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentModelUsage _contentModelUsage;
        private readonly IContentRepository _contentRepository;

        public RepublishAllImages(IContentTypeRepository contentTypeRepository, IContentModelUsage contentModelUsage, IContentRepository contentRepository)
        {
            _contentTypeRepository = contentTypeRepository;
            _contentModelUsage = contentModelUsage;
            _contentRepository = contentRepository;
        }
        
        public override string Execute()
        {
            var contentType = _contentTypeRepository.Load(typeof(Image));
            var imageUsages = _contentModelUsage.ListContentOfContentType(contentType)
                .Select(x => x.ContentLink.ID)
                .Distinct()
                .Select(x => new ContentReference(x))
                .ToList();

            foreach (var imageLink in imageUsages)
            {
                var image = _contentRepository.Get<Image>(imageLink);
                image = (Image) image.CreateWritableClone();
                _contentRepository.Save(image, SaveAction.Publish | SaveAction.ForceNewVersion, AccessLevel.NoAccess);
            }

            return $"Rebuplished {imageUsages.Count} images";
        }
    }
```
