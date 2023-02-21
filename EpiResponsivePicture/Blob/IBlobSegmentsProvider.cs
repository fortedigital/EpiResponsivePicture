using Microsoft.AspNetCore.Http;

namespace Forte.EpiResponsivePicture.Blob;

public interface IBlobSegmentsProvider
{
    bool IsMatch(HttpContext context);
}