namespace Forte.EpiResponsivePicture.ResizedImage.Background
{
    public class PictureSize
    {
        public PictureSize(string mediaCondition, int imageWidth)
        {
            this.MediaCondition = mediaCondition;
            this.ImageWidth = imageWidth;
        }

        public string MediaCondition { get; }
        public int ImageWidth { get;  }
    }
}