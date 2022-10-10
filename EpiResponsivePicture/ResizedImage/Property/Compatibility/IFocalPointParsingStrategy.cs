namespace Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility;

public interface IFocalPointParsingStrategy
{
    FocalPoint Parse(string focalPointBackingString);
}
