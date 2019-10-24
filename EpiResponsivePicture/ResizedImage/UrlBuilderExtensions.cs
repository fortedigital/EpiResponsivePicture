using System;
using EPiServer;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public static class UrlBuilderExtensions
    {
        public static UrlBuilder Add(this UrlBuilder target, string key, string value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (!target.IsEmpty)
                target.QueryCollection.Add(key, value);
            return target;
        }
        
        public static UrlBuilder Width(this UrlBuilder target, int width)
        {
            return target.Add("w", width.ToString());
        }

        public static UrlBuilder Height(this UrlBuilder target, int height)
        {
            return target.Add("h", height.ToString());
        }

        public static UrlBuilder MaxHeight(this UrlBuilder target, int maxHeight)
        {
            return target.Add("maxheight", maxHeight.ToString());
        }

        public static UrlBuilder Quality(this UrlBuilder target, int quality)
        {
            return target.Add("quality", quality.ToString());
        }

        public static UrlBuilder Mode(this UrlBuilder target, ScaleMode mode)
        {
            if (mode == ScaleMode.Default)
            {
                return target;
            }

            return target.Add("mode", mode.ToString().ToLowerInvariant());
         
        }
    }
}