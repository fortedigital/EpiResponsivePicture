namespace Forte.EpiResponsivePicture.ResizedImage
{
    public class PictureProfile
    {
        public int DefaultWidth { get; set; }
        public int[] SrcSetWidths { get; set; }
        public string[] SrcSetSizes { get; set; }
        public int? MaxHeight { get; set; }
        public ScaleMode Mode { get; set; }
        public int? Quality { get; set; }
    }
}