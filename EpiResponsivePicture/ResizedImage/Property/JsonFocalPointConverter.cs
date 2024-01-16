using System;
using EPiServer.ServiceLocation;
using Forte.EpiResponsivePicture.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Forte.EpiResponsivePicture.ResizedImage.Property;

[ServiceConfiguration(typeof(JsonConverter))]
public class JsonFocalPointConverter : JsonConverter
{
    private EpiResponsivePicturesOptions configuration;

    public JsonFocalPointConverter(IOptions<EpiResponsivePicturesOptions> options)
    {
        configuration = options.Value;
    }
    
    public override bool CanConvert(Type objectType)
    {
        return typeof(FocalPoint).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        if (reader.ValueType == null)
            return null;

        if (!typeof(string).IsAssignableFrom(reader.ValueType)
            || !typeof(FocalPoint).IsAssignableFrom(objectType))
            throw new ArgumentException("Invalid type of data for Focal Point");

        var value = (string) reader.Value;
        return FocalPoint.Parse(value, configuration);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var focalPoint = (FocalPoint) value;
        writer.WriteValue(focalPoint.ToString());
    }
}
