using System.Collections.Generic;

namespace Forte.EpiResponsivePicture.Configuration;

public class EpiResponsivePicturesOptions
{
    public bool ImageResizerCompatibilityEnabled { get; set; }
    public List<string> AdditionalSegments { get; set; }
}
