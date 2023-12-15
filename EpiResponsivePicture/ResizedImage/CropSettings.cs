using Reinforced.Typings.Attributes;

namespace Forte.EpiResponsivePicture.ResizedImage;

[TsInterface]
public class CropSettings
{
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }

    public override string ToString()
    {
        return $"{X1},{Y1},{X2},{Y2}";
    }
}