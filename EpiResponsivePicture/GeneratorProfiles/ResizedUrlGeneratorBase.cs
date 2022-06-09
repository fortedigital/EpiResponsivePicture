using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using EPiServer;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.GeneratorProfiles
{
    public abstract class ResizedUrlGeneratorBase : IResizedUrlGenerator
    {
        protected UrlBuilder Builder;

        private readonly Queue<Func<int, PictureSource, FocalPoint, (string Key, string Value)>> CustomQueryRegistrations = new();
        private readonly NameValueCollection CustomQueries = new();
        public UrlBuilder GenerateUrl(string imageUrl, int width, PictureSource pictureSource, FocalPoint focalPoint, ResizedImageFormat format)
        {
            Builder = new UrlBuilder(imageUrl);
            
            Builder.Add(WidthQuery(width));
            Builder.Add(HeightQuery(width, pictureSource));
            Builder.Add(QualityQuery(pictureSource));
            
            if(format != ResizedImageFormat.Preserve)
                Builder.Add(FormatQuery(format));
            
            foreach (var customQueryRegistration in CustomQueryRegistrations)
            {
                AddCustomQuery(customQueryRegistration.Invoke(width, pictureSource, focalPoint));
            }
            
            Builder.Add(CustomQueries);
            
            CustomQueries.Clear();

            return Builder;
        }
        
        protected abstract (string Key, string Value) WidthQuery(int width);
        protected abstract (string Key, string Value) HeightQuery(int width, PictureSource source);
        protected abstract (string Key, string Value) QualityQuery(PictureSource source);
        protected abstract (string Key, string Value) FormatQuery(ResizedImageFormat format);

        protected void RegisterCustomQuery(Func<int, PictureSource, FocalPoint, (string Key, string Value)> registration) =>
            CustomQueryRegistrations.Enqueue(registration);
        private void AddCustomQuery((string Key, string Value) query)
        {
            CustomQueries.Add(query.Key, query.Value);
        }
    }
}