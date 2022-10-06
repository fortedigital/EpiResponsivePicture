using System.Globalization;
using System.Linq;

namespace Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility;

public class ForteResponsivePictureFocalPointParsingStrategy : IFocalPointParsingStrategy
{
    public FocalPoint Parse(string focalPointBackingString)
    {
        var parsed = 
            focalPointBackingString.Split('|')
                .Select(s => s.Replace(',', '.'))
                .Select(s => double.Parse(s, CultureInfo.InvariantCulture))
                .ToList();
        
        return new FocalPoint(parsed[0], parsed[1]);
    }
}
