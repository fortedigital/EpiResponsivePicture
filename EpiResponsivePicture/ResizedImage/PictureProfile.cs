namespace Forte.EpiResponsivePicture.ResizedImage
{
    public class PictureProfile
    {
        public int DefaultWidth { get; set; }
        public int MaxImageDimension { get; set; } = 3200; // Default max served by ImageResizer
        public ResizedImageFormat Format { get; set; } = ResizedImageFormat.Preserve;
        public PictureSource[] Sources { get; set; }
    }
}