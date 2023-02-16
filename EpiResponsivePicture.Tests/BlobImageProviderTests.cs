using Forte.EpiResponsivePicture.Blob;
using NUnit.Framework;

namespace Forte.EpiResponsivePicture.Tests;

[TestFixture]
public class BlobImageProviderTests
{
    [Test]
    public void AddSegments_ShouldNotThrowException_WhenNullIsGiven()
    {
        // When & Then
        Assert.DoesNotThrow(() => BlobImageProvider.AddSegments(null));
    }
}