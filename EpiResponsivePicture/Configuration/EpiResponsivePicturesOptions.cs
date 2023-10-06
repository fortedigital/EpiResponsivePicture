using System.Collections.Generic;

namespace Forte.EpiResponsivePicture.Configuration;

public class EpiResponsivePicturesOptions
{
    public bool ImageResizerCompatibilityEnabled { get; set; }
    public IEnumerable<string> AdditionalSegments { get; set; }
    public int? MaxPictureSize { get; set; } = null;
}
