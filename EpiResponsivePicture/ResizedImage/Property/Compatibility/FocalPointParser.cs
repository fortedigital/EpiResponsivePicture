using System;
using System.Collections.Generic;
using System.Linq;
using Forte.EpiResponsivePicture.Configuration;
using Microsoft.Extensions.Options;

namespace Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility;

public class FocalPointParser
{
    private readonly EpiResponsivePicturesOptions configuration;
    private List<IFocalPointParsingStrategy> parsingStrategies = new();

    public FocalPointParser(IOptions<EpiResponsivePicturesOptions> options)
    {
        configuration = options.Value;
        
        if(configuration.ImageResizerCompatibilityEnabled)
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
