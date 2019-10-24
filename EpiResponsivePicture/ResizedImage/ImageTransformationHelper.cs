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
        public static UrlBuilder ResizedImageUrl(this HtmlHelper helper, ContentReference image, int width, int? height = null, PictureProfile profile = null)
        {
            var baseUrl = ResolveImageUrl(image);
            
            return BuildResizedImageUrl(baseUrl, width, height, profile);
        }
        
        private static string ResolveImageUrl(ContentReference image)
        {
            return UrlResolver.Current.GetUrl(image);
        }
        
        private static UrlBuilder BuildResizedImageUrl(string imageUrl, int? width, int? height, PictureProfile profile)
        {
            var target = new UrlBuilder(imageUrl);

            if (width.HasValue)
                target.Width(width.Value);

            if (height.HasValue)
                target.Height(height.Value);

            if (profile == null) return target;
            
            if (profile.MaxHeight.HasValue)
                target.MaxHeight(profile.MaxHeight.Value);

            if (profile.Mode != ScaleMode.Default)
                target.Mode(profile.Mode);

            if (profile.Quality.HasValue)
                target.Quality(profile.Quality.Value);

            return target;
        }
        
        public static MvcHtmlString ResizedPicture(
            this HtmlHelper helper,
            ContentReference image,
            PictureProfile profile,
            ResizedPictureViewModel resizedPictureViewModel = null,
            string fallbackUrl = null
            )
        {
            var isEmpty = ContentReference.IsNullOrEmpty(image);
            var baseUrl = isEmpty ? fallbackUrl : ResolveImageUrl(image);

            var alternateText =
                ServiceLocator.Current.GetInstance<IContentLoader>().TryGet<ImageBase>(image, out var content)
                    ? content.Description
                    : string.Empty;

            if (resizedPictureViewModel == null)
            {
                resizedPictureViewModel = new ResizedPictureViewModel();
            }
                
            if (!resizedPictureViewModel.ImgElementAttributes.ContainsKey("alt"))
            {
                resizedPictureViewModel.ImgElementAttributes.Add("alt", alternateText);
            }

            return GenerateResizedPicture(baseUrl, profile, resizedPictureViewModel);
        }
        
         public static MvcHtmlString ResizedPicture(this HtmlHelper helper, 
             ResizedPictureViewModel pictureModel, 
             PictureProfile profile, 
             string fallbackUrl = null 
             )
         {
            var isEmpty = string.IsNullOrWhiteSpace(pictureModel.Url);
            var baseUrl = isEmpty ? fallbackUrl : pictureModel.Url;

            return GenerateResizedPicture(baseUrl, profile, pictureModel);
         }
        
        private static MvcHtmlString GenerateResizedPicture(string imageBaseUrl,
            PictureProfile profile, ResizedPictureViewModel pictureModel)
        {
            var urlBuilder = BuildResizedImageUrl(imageBaseUrl, profile.DefaultWidth, null, profile);
            var sourceSets = profile.SrcSetWidths.Select(w => FormatSourceSet(imageBaseUrl, w, profile)).ToArray();

            return GeneratePictureElement(profile, urlBuilder.ToString(), sourceSets, pictureModel);
        }
        
        private static MvcHtmlString GeneratePictureElement(PictureProfile profile,
            string imgUrl,
            string[] sourceSets,
            ResizedPictureViewModel pictureModel)
        {
            var sourceElement = new TagBuilder("source")
            {
                Attributes = {{"srcset", string.Join(", ", sourceSets)}}
            };

            if (profile.SrcSetSizes != null && profile.SrcSetSizes.Length != 0)
                sourceElement.Attributes.Add("sizes", string.Join(", ", profile.SrcSetSizes));

            var pictureElement = new TagBuilder("picture")
            {
                InnerHtml = sourceElement.ToString(TagRenderMode.SelfClosing) +
                            CreateImgElement(imgUrl, pictureModel.ImgElementAttributes)
                                .ToString(TagRenderMode.SelfClosing)
            };

            foreach (var kv in pictureModel.PictureElementAttributes)
            {
                pictureElement.Attributes.Add(kv.Key, kv.Value);
            }

            return new MvcHtmlString(pictureElement.ToString());
        }
        
        private static TagBuilder CreateImgElement(string imgUrl, IDictionary<string, string> imgAttributes)
        {
            var imgAttributesConcat = imgAttributes.Concat(new Dictionary<string, string>
            {
                {"src", imgUrl},
                {"data-object-fit", "cover"},
                {"data-object-position", "center"}
            });

            var imgElement = new TagBuilder("img");

            foreach (var kv in imgAttributesConcat)
            {
                imgElement.Attributes.Add(kv.Key, kv.Value);
            }

            return imgElement;
        }
        
        private static string FormatSourceSet(string imageUrl, int width, PictureProfile profile)
        {
            var url = BuildResizedImageUrl(imageUrl, width, null, profile);
            return $"{url} {width}w";
        }
    }
}
