using System;
using System.Collections.Specialized;
using EPiServer;
using Microsoft.Toolkit.Diagnostics;

namespace Forte.EpiResponsivePicture.ResizedImage;

public static partial class UrlBuilderExtensions
{
    public static UrlBuilder Clone(this UrlBuilder builder)
    {
        return new UrlBuilder(builder.ToString());
    }

    public static UrlBuilder Add(this UrlBuilder target, string key, string value)
    {
        Guard.IsNotNull(target, nameof(target));
        if (!target.IsEmpty)
            target.QueryCollection.Add(key, value);
        return target;
    }
        
    public static UrlBuilder Add(this UrlBuilder target, (string Key, string Value) queryTuple)
    {
        Guard.IsNotNull(target, nameof(target));
        if (!target.IsEmpty)
            target.QueryCollection.Add(queryTuple.Key, queryTuple.Value);
        return target;
    }

    public static UrlBuilder Add(this UrlBuilder target, NameValueCollection collection)
    {
        Guard.IsNotNull(target, nameof(target));
        if (!target.IsEmpty)
            target.QueryCollection.Add(collection);
        return target;
    }

    public static UrlBuilder Remove(this UrlBuilder target, string key)
    {
        Guard.IsNotNull(target, nameof(target));
        if (!target.IsEmpty && target.QueryCollection[key] != null)
            target.QueryCollection.Remove(key);
        return target;
    }
}
