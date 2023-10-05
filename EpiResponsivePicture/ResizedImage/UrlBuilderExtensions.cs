using EPiServer;
using Microsoft.Toolkit.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;

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

    public static UrlBuilder Remove(this UrlBuilder target, IEnumerable<string> keys)
    {
        Guard.IsNotNull(target, nameof(target));
        Guard.IsNotNull(keys, nameof(keys));
        foreach (var key in keys)
        {
            target.Remove(key);
        }
        return target;
    }

    public static UrlBuilder AddOrOverride(this UrlBuilder target, string key, string value)
        => target.Remove(key).Add(key, value);

    public static UrlBuilder AddOrOverride(this UrlBuilder target, (string Key, string Value) queryTuple)
        => target.Remove(queryTuple.Key).Add(queryTuple);

    public static UrlBuilder AddOrOverride(this UrlBuilder target, NameValueCollection collection)
        => target.Remove(collection.AllKeys).Add(collection);
}
