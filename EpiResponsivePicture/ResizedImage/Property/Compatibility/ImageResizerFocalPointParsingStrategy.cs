using System;
using Newtonsoft.Json;

namespace Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility;

public class ImageResizerFocalPointParsingStrategy : IFocalPointParsingStrategy
{
    public FocalPoint Parse(string focalPointBackingString)
    {
        var values = new { x = 0d, y = 0d };

        var parsed = JsonConvert.DeserializeAnonymousType(focalPointBackingString, values);
        
        return new FocalPoint(Math.Round(parsed.x / 100, 2), Math.Round(parsed.y / 100, 2));
    }
}
