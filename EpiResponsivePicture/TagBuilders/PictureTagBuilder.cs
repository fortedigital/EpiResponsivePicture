using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Toolkit.Diagnostics;

namespace Forte.EpiResponsivePicture.TagBuilders
{
    public class PictureTagBuilder : IPictureTagBuilder
    {
        private readonly TagBuilder element;
        private PictureProfile pictureProfile;
        private ContentReference pictureContentReference;
        private string pictureFallbackUrl;
        private ResizedPictureViewModel pictureViewModel;
        private FocalPoint focalPoint;
        private string pictureUrl;
        private string altImgText;
        
        private PictureTagBuilder()
        {
            element = new TagBuilder("picture");
        }

        public static IPictureTagBuilder Create() => new PictureTagBuilder();

        public IPictureTagBuilder WithProfile(PictureProfile profile)
        {
            pictureProfile = profile;
            return this;
        }

        public IPictureTagBuilder WithContentReference(ContentReference contentReference)
        {
            pictureContentReference = contentReference;
            return this;
        }

        public IPictureTagBuilder WithFallbackUrl(string fallbackUrl)
        {
            pictureFallbackUrl = fallbackUrl;
            return this;
        }

        public IPictureTagBuilder WithViewModel(ResizedPictureViewModel viewModel)
        {
            pictureViewModel = viewModel;
            return this;
        }

        public TagBuilder Build()
        {
            Guard.IsNotNull(pictureProfile, nameof(pictureProfile));
            
            pictureFallbackUrl ??= string.Empty;
            pictureViewModel ??= new ResizedPictureViewModel();
            
            GetPictureData();

            var sourceTagBuilder = SourceTagBuilder
                .Create()
                .WithImageUrl(pictureUrl)
                .WithFocalPoint(focalPoint)
                .WithResizedImageFormat(pictureProfile.Format);

            foreach (var pictureSource in pictureProfile.Sources)
            {
                var sourceTag = sourceTagBuilder.WithSource(pictureSource).Build();

                element.InnerHtml.AppendHtml(sourceTag.RenderSelfClosingTag());
                
                sourceTagBuilder.Clear();
            }
            
            AddAdditionalAttributes();

            var imgTag = CreateImageElement();
            element.InnerHtml.AppendHtml(imgTag.RenderSelfClosingTag());
            
            return element;
        }
        
        private void GetPictureData()
        {
            var imageFound = ServiceLocator.Current.GetInstance<IContentLoader>().TryGet<IContentData>(pictureContentReference,
                new LoaderOptions { LanguageLoaderOption.FallbackWithMaster() }, out var content) && content is IImage;
            
            if(imageFound)
                GetImgAltText((IImage) content);

            GetFocalPoint(content as IResponsiveImage);
            SetPictureProfilesAspectRatios(content as IResponsiveImage);

            pictureUrl = imageFound ? ResolveUrl() : pictureFallbackUrl;
        }

        private void GetImgAltText(IImage image)
        {
            altImgText = image.Description ?? string.Empty;
        }
        
        private void GetFocalPoint(IResponsiveImage responsiveImage)
        {
            focalPoint = responsiveImage?.FocalPoint ?? FocalPoint.Center;
        }

        private void SetPictureProfilesAspectRatios(IResponsiveImage responsiveImage)
        {
            foreach (var pictureProfileSource in pictureProfile.Sources)
            {
                if (responsiveImage is null || pictureProfileSource.TargetAspectRatio is null)
                {
                    pictureProfileSource.TargetAspectRatio = AspectRatio.Default; // 4:3 
                    continue;                    
                }
                
                if(pictureProfileSource.TargetAspectRatio == AspectRatio.Original)
                    pictureProfileSource.TargetAspectRatio = AspectRatio.Create((double) responsiveImage.Width / responsiveImage.Height);
            }
        }

        private string ResolveUrl()
        {
            var urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
            return urlResolver.GetUrl(pictureContentReference);
        }

        private void AddAdditionalAttributes()
        {
            foreach (var (key, value) in pictureViewModel.PictureElementAttributes)
            {
                element.Attributes.Add(key, value);
            }
        }

        private TagBuilder CreateImageElement()
        {
            var imgTagBuilder = new TagBuilder("img");
            
            imgTagBuilder.Attributes.Add("src", pictureUrl);
            imgTagBuilder.Attributes.Add("alt", altImgText);
            
            foreach (var imgElementAttribute in pictureViewModel.ImgElementAttributes)
            {
                imgTagBuilder.Attributes.Add(imgElementAttribute);
            }

            return imgTagBuilder;
        }
    }
}