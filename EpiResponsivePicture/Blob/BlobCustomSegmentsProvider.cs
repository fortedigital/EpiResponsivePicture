using System;
using System.Collections.Generic;
using Forte.EpiResponsivePicture.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Forte.EpiResponsivePicture.Blob;

public class BlobCustomSegmentsProvider : IBlobSegmentsProvider
{
    private List<string> segments = new()
    {
        "/contentassets", "/globalassets", "/siteassets"
    };
    
    public BlobCustomSegmentsProvider(IOptions<EpiResponsivePicturesOptions> options)
    {
        if (options.Value.AdditionalSegments != null)
            segments.AddRange(options.Value.AdditionalSegments);
    }

    public bool IsMatch(HttpContext context)
    {
        foreach (var segment in segments)
        {
            if (context.Request.Path.StartsWithSegments(segment, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}