using System;
using System.Collections.Generic;
using System.Linq;
using Forte.EpiResponsivePicture.Configuration;

namespace Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility;

public class FocalPointParser
{
    private List<IFocalPointParsingStrategy> parsingStrategies = new();

    public FocalPointParser(EpiResponsivePicturesOptions options)
    {
        if(options.ImageResizerCompatibilityEnabled)
            parsingStrategies.Add(new ImageResizerFocalPointParsingStrategy());
        
        parsingStrategies.Add(new ForteResponsivePictureFocalPointParsingStrategy());
    }

    public FocalPoint Parse(string focalPointBackingString)
    {
        foreach (var parsingStrategy in parsingStrategies)
        {
            FocalPoint focalPoint;
            try
            {
                focalPoint = parsingStrategy.Parse(focalPointBackingString); 
            }
            catch
            {
                continue;
                // ignored
            }
            
            return focalPoint;
        }

        throw new InvalidOperationException($"Cannot parse: {focalPointBackingString} using any of following strategies: {string.Join(',', parsingStrategies.Select(x => x.GetType().Name))}");
    }
    
}
