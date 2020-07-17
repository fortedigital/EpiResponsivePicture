using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public class ImageBase : ImageData, IImage, IResponsiveImage, ILocalizable
    {
        public override Blob BinaryData
        {
            // Source: https://gregwiechec.com/2015/07/localizable-media-assets/
            get
            {
                if (ContentReference.IsNullOrEmpty(ContentLink)) return base.BinaryData;
                if (Language.Name == MasterLanguage.Name) return base.BinaryData;

                var content = GetMasterLanguageImage();
                return content.BinaryData;
            }
            set => base.BinaryData = value;
        }

        [ImageDescriptor(Height = 48, Width = 48)]
        public override Blob Thumbnail
        {
            get
            {
                if (ContentReference.IsNullOrEmpty(ContentLink)) return null;
                if (Language.Name == MasterLanguage.Name) return base.Thumbnail;

                var content = GetMasterLanguageImage();
                return content.Thumbnail;
            }
            set => base.Thumbnail = value;
        }

        [CultureSpecific]
        [Editable(true)]
        [Display(
            Name = "Alternate text",
            Description = "Description of the image")]
        public virtual string Description { get; set; }

        public virtual IEnumerable<CultureInfo> ExistingLanguages { get; set; }
        public virtual CultureInfo MasterLanguage { get; set; }

        [Display(Name = "Focal point")]
        [BackingType(typeof(PropertyFocalPoint))]
        public virtual FocalPoint FocalPoint { get; set; }

        [ScaffoldColumn(false), Editable(false)]
        public virtual int Width { get; set; }

        [ScaffoldColumn(false), Editable(false)]
        public virtual int Height { get; set; }

        private ImageBase GetMasterLanguageImage()
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var masterLanguageImage = contentRepository.Get<ImageBase>(ContentLink.ToReferenceWithoutVersion(),
                new LanguageSelector(MasterLanguage.Name));
            return masterLanguageImage;
        }
    }
}