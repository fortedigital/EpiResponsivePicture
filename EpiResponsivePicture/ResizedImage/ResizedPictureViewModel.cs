using System.Collections.Generic;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public class ResizedPictureViewModel
    {
            public ResizedPictureViewModel()
            {
                this.ImgElementAttributes = new Dictionary<string, string>();
                this.PictureElementAttributes = new Dictionary<string, string>();
            }
        
            public string Url { get; set; }
            
            public IDictionary<string, string> ImgElementAttributes { get; }
            public IDictionary<string, string> PictureElementAttributes { get; }
        }
    }
