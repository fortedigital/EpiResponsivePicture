using System;
using EPiServer;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    [Obsolete]
    public static partial class UrlBuilderExtensions
    {
        [Obsolete]
        public static UrlBuilder Width(this UrlBuilder target, int width)
        {
            return target.Add("w", width.ToString());
        }
        
        [Obsolete]
        public static UrlBuilder Height(this UrlBuilder target, int height)
        {
            return target.Add("h", height.ToString());
        }
        
        [Obsolete]
        public static UrlBuilder Quality(this UrlBuilder target, int quality)
        {
            return target.Add("quality", quality.ToString());
        }
        
        [Obsolete]
        public static UrlBuilder Crop(this UrlBuilder target, CropSettings settings)
        {
            return target.Add("crop", settings.ToString());
        }
        
        [Obsolete]
        public static UrlBuilder Zoom(this UrlBuilder target, double zoom)
        {
            return target.Add("zoom", zoom.ToString("0.##"));
        }
        
        [Obsolete]
        public static UrlBuilder Mode(this UrlBuilder target, ScaleMode mode)
        {
            if (mode == ScaleMode.Default) return target;

            return target.Add("mode", mode.ToString().ToLowerInvariant());
        }
        
        [Obsolete]
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