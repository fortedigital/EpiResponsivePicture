namespace Forte.EpiResponsivePicture.ResizedImage.Background;

public class BackgroundPictureProfile
{
    public ResizedImageFormat Format { get; set; } = ResizedImageFormat.Preserve;
    public PictureSize[] AllowedSizes { get; set; }
}