using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public static class ImageTransformationHelper
    {
        public static UrlBuilder ResizedImageUrl(this HtmlHelper helper, ContentReference image, int width,
            ResizedImageFormat? format = null)
        {
            var baseUrl = ResolveImageUrl(image);

            var target = new UrlBuilder(baseUrl);
            target.Width(width);
            if (format.HasValue)
                target.Format(format.Value);
            return target;
        }

        public static MvcHtmlString ResizedPicture(this HtmlHelper helper,
            ContentReference image,
            PictureProfile profile,
            string fallbackUrl = null,
            ResizedPictureViewModel pictureModel = null)
        {
            if (pictureModel == null) pictureModel = new ResizedPictureViewModel();

            var imageFound = ServiceLocator.Current.GetInstance<IContentLoader>()
                                 .TryGet<IContentData>(image, new LoaderOptions{ LanguageLoaderOption.FallbackWithMaster() }, 
                                     out var content) &&
                             content is IImage;

            var baseUrl = imageFound
                ? ResolveImageUrl(image)
                : fallbackUrl;

            if (!pictureModel.ImgElementAttributes.ContainsKey("alt"))
            {
                var alternateText = imageFound
                    ? ((IImage) content).Description
                    : string.Empty;
                pictureModel.ImgElementAttributes.Add("alt", alternateText);
            }

            return GenerateResizedPicture(baseUrl, profile, content as IResponsiveImage, pictureModel);
        }

        private static MvcHtmlString GenerateResizedPicture(string imageBaseUrl,
            PictureProfile profile, IResponsiveImage image, ResizedPictureViewModel pictureModel)
        {
            return GeneratePictureElement(profile, imageBaseUrl, image, pictureModel);
        }

        private static MvcHtmlString GeneratePictureElement(PictureProfile profile,
            string imgUrl,
            IResponsiveImage focalPoint, ResizedPictureViewModel pictureModel)
        {
            var sourceElements = profile.Sources?.Select(x => CreateSourceElement(imgUrl, x, focalPoint,
                profile.MaxImageDimension, profile.Format));

            var pictureElement = new TagBuilder("picture")
            {
                InnerHtml =
                    string.Join(string.Empty,
                        sourceElements?.Select(x => x.ToString(TagRenderMode.SelfClosing)) ?? new string[0]) +
                    CreateImgElement(
                            BuildResizedImageUrl(imgUrl, profile.DefaultWidth, ScaleMode.Default, AspectRatio.Original,
                                null, focalPoint, null, profile.Format).ToString(), pictureModel.ImgElementAttributes)
                        .ToString(TagRenderMode.SelfClosing)
            };

            foreach (var kv in pictureModel.PictureElementAttributes)
            {
                pictureElement.Attributes.Add(kv.Key, kv.Value);
            }

            return new MvcHtmlString(pictureElement.ToString());
        }

        private static TagBuilder CreateSourceElement(string imageUrl, PictureSource source,
            IResponsiveImage focalPoint, int maxImageDimension, ResizedImageFormat? format)
        {
            var srcSets = source.AllowedWidths
                .Select(width => BuildSize(imageUrl, width, source.Mode, source.TargetAspectRatio, source.Quality,
                    focalPoint, maxImageDimension, format));

            var tagBuilder = new TagBuilder("source")
            {
                Attributes =
                {
                    {"media", $"{source.MediaCondition}"},
                    {"srcset", string.Join(", ", srcSets)},
                    {"sizes", string.Join(", ", source.Sizes)}
                }
            };

            return tagBuilder;
        }

        private static string BuildSize(string imageUrl, int width, ScaleMode sourceMode,
            AspectRatio sourceTargetAspectRatio, int? sourceQuality, IResponsiveImage focalPoint, int maxImageDimension,
            ResizedImageFormat? format)
        {
            var url = BuildResizedImageUrl(imageUrl, width, sourceMode, sourceTargetAspectRatio, sourceQuality,
                focalPoint,
                maxImageDimension,
                format);

            return $"{url} {width}w";
        }

        private static TagBuilder CreateImgElement(string imgUrl, IDictionary<string, string> attributes)
        {
            var tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("src", imgUrl);

            foreach (var attribute in attributes) tagBuilder.Attributes.Add(attribute);

            return tagBuilder;
        }

        private static string ResolveImageUrl(ContentReference image)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
            return urlResolver.GetUrl(image);
        }

        private static UrlBuilder BuildResizedImageUrl(string imageUrl, int width,
            ScaleMode scaleMode, AspectRatio targetAspectRatio, int? quality,
            IResponsiveImage image, int? maxImageDimension, ResizedImageFormat? format)
        {
            width = Math.Min(width, maxImageDimension ?? int.MaxValue);

            var target = new UrlBuilder(imageUrl);

            if (scaleMode != ScaleMode.Default)
                target.Mode(scaleMode);

            if (quality.HasValue)
                target.Quality(quality.Value);

            if (format.HasValue)
                target.Format(format.Value);

            if (scaleMode != ScaleMode.Default && scaleMode != ScaleMode.Max
                                               && image != null)
            {
                if (targetAspectRatio == null || !targetAspectRatio.HasValue)
                    throw new ArgumentException("Aspect ratio is required when ScaleMode is other than Max");

                var height = (int) (width / targetAspectRatio.Ratio);
                if (height > maxImageDimension)
                {
                    height = maxImageDimension.Value;
                    width = (int) (height * targetAspectRatio.Ratio);
                }

                target.Width(width);
                target.Height(height);

                var cropSettings = ImageCropper.GetCropSettings(targetAspectRatio, image);
                if (cropSettings != null)
                    target.Crop(cropSettings);
            }
            else
            {
                target.Width(width);
            }

            return target;
        }
    }
}