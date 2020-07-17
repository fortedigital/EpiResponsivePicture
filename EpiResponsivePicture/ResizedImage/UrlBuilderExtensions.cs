using System;
using System.Collections.Specialized;
using EPiServer;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public static class UrlBuilderExtensions
    {
        public static UrlBuilder Clone(this UrlBuilder builder)
        {
            return new UrlBuilder(builder.ToString());
        }

        public static UrlBuilder Add(this UrlBuilder target, string key, string value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (!target.IsEmpty)
                target.QueryCollection.Add(key, value);
            return target;
        }

        public static UrlBuilder Add(this UrlBuilder target, NameValueCollection collection)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (!target.IsEmpty)
                target.QueryCollection.Add(collection);
            return target;
        }

        public static UrlBuilder Remove(this UrlBuilder target, string key)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (!target.IsEmpty && target.QueryCollection[key] != null)
                target.QueryCollection.Remove(key);
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

        public static UrlBuilder Quality(this UrlBuilder target, int quality)
        {
            return target.Add("quality", quality.ToString());
        }

        public static UrlBuilder Crop(this UrlBuilder target, CropSettings settings)
        {
            return target.Add("crop", settings.ToString());
        }

        public static UrlBuilder Zoom(this UrlBuilder target, double zoom)
        {
            return target.Add("zoom", zoom.ToString("0.##"));
        }

        public static UrlBuilder Mode(this UrlBuilder target, ScaleMode mode)
        {
            if (mode == ScaleMode.Default) return target;

            return target.Add("mode", mode.ToString().ToLowerInvariant());
        }

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