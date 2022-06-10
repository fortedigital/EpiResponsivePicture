using System;
using EPiServer;
using Forte.EpiResponsivePicture.TagBuilders;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public static partial class UrlBuilderExtensions
    {
        [Obsolete($"Provider specific, use {nameof(PictureTagBuilder)} instead")]
        public static UrlBuilder Width(this UrlBuilder target, int width)
        {
            return target.Add("w", width.ToString());
        }
        
        [Obsolete($"Provider specific, use {nameof(PictureTagBuilder)} instead")]
        public static UrlBuilder Height(this UrlBuilder target, int height)
        {
            return target.Add("h", height.ToString());
        }
        
        [Obsolete($"Provider specific, use {nameof(PictureTagBuilder)} instead")]
        public static UrlBuilder Quality(this UrlBuilder target, int quality)
        {
            return target.Add("quality", quality.ToString());
        }
        
        [Obsolete($"Provider specific, use {nameof(PictureTagBuilder)} instead")]
        public static UrlBuilder Crop(this UrlBuilder target, CropSettings settings)
        {
            return target.Add("crop", settings.ToString());
        }
        
        [Obsolete($"Provider specific, use {nameof(PictureTagBuilder)} instead")]
        public static UrlBuilder Zoom(this UrlBuilder target, double zoom)
        {
            return target.Add("zoom", zoom.ToString("0.##"));
        }
        
        [Obsolete($"Provider specific, use {nameof(PictureTagBuilder)} instead")]
        public static UrlBuilder Mode(this UrlBuilder target, ScaleMode mode)
        {
            if (mode == ScaleMode.Default) return target;

            return target.Add("mode", mode.ToString().ToLowerInvariant());
        }
        
        [Obsolete($"Provider specific, use {nameof(PictureTagBuilder)} instead")]
        public static UrlBuilder Format(this UrlBuilder target, ResizedImageFormat format)
        {
            if (format == ResizedImageFormat.Preserve)
                target.QueryCollection.Remove("format");
            else
                target.Add("format", format.ToString().ToLowerInvariant());
            return target;
        }
    }
}