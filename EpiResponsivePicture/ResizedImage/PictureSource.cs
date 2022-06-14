namespace Forte.EpiResponsivePicture.ResizedImage;

public class PictureSource
{
    public string MediaCondition { get; set; }
    public int[] AllowedWidths { get; set; }
    public string[] Sizes { get; set; }
    public ScaleMode Mode { get; set; }
    public AspectRatio TargetAspectRatio { get; set; }
    public int? Quality { get; set; }
}
