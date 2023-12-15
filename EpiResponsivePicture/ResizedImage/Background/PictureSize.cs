namespace Forte.EpiResponsivePicture.ResizedImage.Background;

public class PictureSize
{
    public PictureSize(string mediaCondition, int imageWidth)
    {
        MediaCondition = mediaCondition;
        ImageWidth = imageWidth;
    }

    public string MediaCondition { get; }
    public int ImageWidth { get; }
}