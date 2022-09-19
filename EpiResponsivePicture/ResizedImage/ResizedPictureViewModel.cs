using System.Collections.Generic;

namespace Forte.EpiResponsivePicture.ResizedImage;

public class ResizedPictureViewModel
{
    public ResizedPictureViewModel()
    {
        ImgElementAttributes = new Dictionary<string, string>();
        PictureElementAttributes = new Dictionary<string, string>();
    }

    public IDictionary<string, string> ImgElementAttributes { get; }
    public IDictionary<string, string> PictureElementAttributes { get; }
}
