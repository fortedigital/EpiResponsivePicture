using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Forte.EpiResponsivePicture.GeneratorProfiles;
using Forte.EpiResponsivePicture.ResizedImage;
using Forte.EpiResponsivePicture.ResizedImage.Property;
using Forte.EpiResponsivePicture.TagBuilders;
using Moq;
using NUnit.Framework;
using static Forte.EpiResponsivePicture.ResizedImage.CssHelpers;

namespace Forte.EpiResponsivePicture.Tests
{
    [TestFixture]
    public class PictureTagBuilderTests
    {
        private const string ResizedUrl = "/images/foo.jpg?QUERIES";
        private const string FallbackUrl = "/images/fallback.jpg";
        private const string ImageUrl = "/images/foo.jpg";
        private Mock<IContentLoader> contentLoaderMock;
        private Mock<IUrlResolver> urlResolverMock;
        private Mock<ContentReference> contentReferenceMock;
        private Mock<IResizedUrlGenerator> resizedUrlGenerator;
        
        
        [SetUp]
        public void Setup()
        {
            contentLoaderMock = new Mock<IContentLoader>();

            contentReferenceMock = new Mock<ContentReference>();

            resizedUrlGenerator = new Mock<IResizedUrlGenerator>();
            resizedUrlGenerator.Setup(generator => generator.GenerateUrl(It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<PictureSource>(), It.IsAny<FocalPoint>(), It.IsAny<ResizedImageFormat>())).Returns(new UrlBuilder(ResizedUrl));

            urlResolverMock = new Mock<IUrlResolver>();
            urlResolverMock
                .Setup(r => r.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(),
                    It.IsAny<UrlResolverArguments>())).Returns(ImageUrl);
            
            ServiceLocator.SetServiceProvider(new DummyServiceLocator(new Dictionary<Type, object>
            {
                {typeof(IContentLoader), contentLoaderMock.Object},
                {typeof(IUrlResolver), urlResolverMock.Object},
                {typeof(IResizedUrlGenerator), resizedUrlGenerator.Object},
            }));
        }

        [Test]
        public void When_PictureProfile_Is_Null_Throw_ArgumentNullException()
        {
            var pictureTagBuilder = PictureTagBuilder.Create().WithProfile(null);

            Assert.Throws<ArgumentNullException>(() => pictureTagBuilder.Build());
        }
        
        [TestCase(ExpectedResult = "<picture><source media=\"(min-width: 1900px)\" sizes=\"90vw\" srcset=\"/images/foo.jpg?QUERIES 1900w, /images/foo.jpg?QUERIES 2400w\" /><source media=\"(min-width: 1000px)\" sizes=\"(min-width: 1400px) 1400px, 100vw\" srcset=\"/images/foo.jpg?QUERIES 1000w, /images/foo.jpg?QUERIES 1200w, /images/foo.jpg?QUERIES 1400w, /images/foo.jpg?QUERIES 1600w\" /><source media=\"(max-width: 1000px)\" sizes=\"50vw\" srcset=\"/images/foo.jpg?QUERIES 1000w, /images/foo.jpg?QUERIES 1200w, /images/foo.jpg?QUERIES 1400w, /images/foo.jpg?QUERIES 1600w\" /><img alt=\"\" src=\"/images/fallback.jpg\" /></picture>")]
        public string Readme_Example_Should_Work()
        {
            var imageContentMock = new Mock<IContentData>().Object; 
            contentLoaderMock.Setup(loader =>
                loader.TryGet(It.IsAny<ContentReference>(), It.IsAny<LoaderOptions>(), out imageContentMock)).Returns(true);

            var profile = new PictureProfile()
            {
                DefaultWidth = 800,
                MaxImageDimension = 2500,
                Sources = new []
                {
                    new PictureSource
                    {
                        MediaCondition = MediaQueryMinWidth(1900),
                        AllowedWidths = new [] {1900,2400},
                        Sizes = new []
                        {
                            Size((90, Unit.Vw))
                        }
                    }, 
                    new PictureSource
                    {
                        MediaCondition = MediaQueryMinWidth(1000),
                        AllowedWidths = new [] {1000,1200, 1400,1600},
                        Mode = ScaleMode.Crop,
                        TargetAspectRatio = AspectRatio.Create(16,9),
                        Sizes = new []
                        {
                            MediaQueryMinWidthWithSize(1400, 1400),
                            Size((100, Unit.Vw)),
                        }
            
                    },
                    new PictureSource
                    {
                        MediaCondition = MediaQueryMaxWidth(1000),
                        AllowedWidths = new [] {1000,1200, 1400,1600},
                        Mode = ScaleMode.Crop,
                        TargetAspectRatio = AspectRatio.Create(1),
                        Quality = 60,
                        Sizes = new []
                        {
                            Size((50, Unit.Vw)),
                        }
                    }
                }
            };
            
            var pictureTagBuilder = PictureTagBuilder
                .Create()
                .WithProfile(profile)
                .WithContentReference(contentReferenceMock.Object)
                .WithFallbackUrl(FallbackUrl);

            using var writer = new StringWriter();
            
            pictureTagBuilder.Build().WriteTo(writer, HtmlEncoder.Default);

            return writer.ToString();
        }
    }
}