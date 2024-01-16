using Forte.EpiResponsivePicture.Configuration;
using Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility;
using Reinforced.Typings.Attributes;

namespace Forte.EpiResponsivePicture.ResizedImage.Property;

[TsInterface(AutoExportProperties = true, AutoExportMethods = false)]
public class FocalPoint
{
    public FocalPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }

    public static FocalPoint Center => new(0.5, 0.5);

    public override string ToString()
    {
        return $"{X:0.###}|{Y:0.###}".Replace(',', '.');
    }

    public static FocalPoint Parse(string input, EpiResponsivePicturesOptions configuration)
    {
        return new FocalPointParser(configuration).Parse(input);
    }
}
