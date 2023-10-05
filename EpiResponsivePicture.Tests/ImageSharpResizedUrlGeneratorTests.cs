using Forte.EpiResponsivePicture.GeneratorProfiles;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;
using NUnit.Framework;
using System.Globalization;

namespace Forte.EpiResponsivePicture.Tests
{
    [TestFixture]
    public class ImageSharpResizedUrlGeneratorTests
    {
        private ImageSharpResizedUrlGenerator _urlGenerator;

        [SetUp]
        public void Setup()
        {
            _urlGenerator = new();
        }

        [Test]
        public void When_BaseUrl_Has_No_Query_Add_New_Values()
        {
            var baseUrl = "https://localhost/globalassets/picture.jpg";
            var width = 600;
            var height = 480;
            var source = new PictureSource()
            {
                TargetAspectRatio = AspectRatio.Create(width / (float)height),
                Mode = ScaleMode.Crop,
                Quality = PictureQuality.Create(80),
            };
            var profile = new PictureProfile()
            {
                DefaultWidth = width,
                Format = ResizedImageFormat.Jpeg,
            };
            var focalPoint = FocalPoint.Center;
            var expectedUrl = GetExpectedUrl(baseUrl, width, height, source, profile, focalPoint);

            var resultUrl = _urlGenerator
                .GenerateUrl(baseUrl, width, source, profile, focalPoint)
                .ToString();

            Assert.AreEqual(expectedUrl, resultUrl);
        }

        [Test]
        public void When_BaseUrl_Already_Has_Query_Override_Duplicates_Add_New_And_Leave_Existing_Values()
        {
            var baseUrl = "https://localhost/globalassets/picture.jpg";
            var leftovers = "leftover=leftover&";
            var existingQueryString = $"?{leftovers}width=1200&height=640&quality=60&quality=60&rmode=Crop";
            var width = 600;
            var height = 480;
            var source = new PictureSource()
            {
                TargetAspectRatio = AspectRatio.Create(width / (float)height),
                Mode = ScaleMode.Crop,
                Quality = PictureQuality.Create(80),
            };
            var profile = new PictureProfile()
            {
                DefaultWidth = width,
                Format = ResizedImageFormat.Jpeg,
            };
            var focalPoint = FocalPoint.Center;
            var expectedUrl = GetExpectedUrl(baseUrl, width, height, source, profile, focalPoint, leftovers);

            var resultUrl = _urlGenerator
                .GenerateUrl($"{baseUrl}{existingQueryString}", width, source, profile, focalPoint)
                .ToString();

            Assert.AreEqual(expectedUrl, resultUrl);
        }

        private static string GetExpectedUrl(string baseUrl, int width, int height, PictureSource source, PictureProfile profile, FocalPoint focalPoint, string leftovers = "")
        {
            return $"{baseUrl}?{leftovers}width={width}&height={height}&quality={source.Quality.Quality}&rxy={focalPoint.X.ToString("0.###", CultureInfo.InvariantCulture)},{focalPoint.Y.ToString("0.###", CultureInfo.InvariantCulture)}&rmode={source.Mode}&format={profile.Format.ToString().ToUpperInvariant()}";
        }
    }
}
