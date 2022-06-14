using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Forte.EpiResponsivePicture.GeneratorProfiles;
using Moq;
using NUnit.Framework;
using Forte.EpiResponsivePicture.ResizedImage;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static Forte.EpiResponsivePicture.ResizedImage.CssHelpers;

namespace Forte.EpiResponsivePicture.Tests
{
    [TestFixture]
    public class ImageTransformationHelperTests
    {
        [SetUp]
        public void Setup()
        {
            var contentLoaderMock = new Mock<IContentLoader>();

            var urlResolverMock = new Mock<IUrlResolver>();
            urlResolverMock
                .Setup(r => r.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(),
                    It.IsAny<UrlResolverArguments>())).Returns("/images/foo.jpg");

            ServiceLocator.SetServiceProvider(new DummyServiceLocator(new Dictionary<Type, object>
            {
                {typeof(IContentLoader), contentLoaderMock.Object},
                {typeof(IUrlResolver), urlResolverMock.Object},
                {typeof(IResizedUrlGenerator), new ImageSharpResizedUrlGenerator() },
            }));
        }

        [TestCase(ResizedImageFormat.Preserve, ExpectedResult = "/images/foo.jpg?width=500")]
        [TestCase(ResizedImageFormat.Jpeg, ExpectedResult = "/images/foo.jpg?width=500&format=JPEG")]
        public string TestResizedImageUrl(ResizedImageFormat format)
        {
            HtmlHelper html = null;

            return html.ResizedImageUrl(new ContentReference(123), 500, format).ToString();
        }

        [Test]
        public void When_PictureProfile_Preserves_ImageFormat_ResizedPicture_Does_Not_Add_Format_To_QueryString()
        {
            HtmlHelper html = null;

            var markup = html.ResizedPicture(new ContentReference(123), profilePreservingFormat, "/images/foo.jpg")
                .ToString();

            Assert.That(markup,
                Is.EqualTo(
                    "<picture><source media=\"(min-width: 1900px)\" sizes=\"90vw\" srcset=\"/images/foo.jpg?width=1900&amp;height=1425&amp;quality=80&amp;rxy=0.5,0.5 1900w, /images/foo.jpg?width=2400&amp;height=1800&amp;quality=80&amp;rxy=0.5,0.5 2400w\" /><img alt=\"\" src=\"/images/foo.jpg\" /></picture>"));
        }

        [Test]
        public void When_PictureProfile_Overrides_ImageFormat_ResizedPicture_Adds_Format_To_QueryString()
        {
            HtmlHelper html = null;

            var markup = html.ResizedPicture(new ContentReference(123), profileOverridingFormat, "/images/foo.jpg")
                .ToString();

            Assert.That(markup,
                Is.EqualTo(
                    "<picture><source media=\"(min-width: 1900px)\" sizes=\"90vw\" srcset=\"/images/foo.jpg?width=1900&amp;height=1425&amp;quality=80&amp;format=JPEG&amp;rxy=0.5,0.5 1900w, /images/foo.jpg?width=2400&amp;height=1800&amp;quality=80&amp;format=JPEG&amp;rxy=0.5,0.5 2400w\" /><img alt=\"\" src=\"/images/foo.jpg\" /></picture>"));
        }

        private static readonly PictureProfile profileOverridingFormat = new()
        {
            Format = ResizedImageFormat.Jpg,
            Sources = new ImmutableArray<PictureSource>
            {
                new()
                {
                    MediaCondition = MediaQueryMinWidth(1900),
                    AllowedWidths = new ImmutableArray<int>() { 1900, 2400 },
                    TargetAspectRatio = AspectRatio.Create(1),
                    Sizes = new ImmutableArray<string> { Size((90, Unit.Vw)) },
                }
            },
        };

        private static readonly PictureProfile profilePreservingFormat = new()
        {
            Sources = new ImmutableArray<PictureSource>()
            {
                new()
                {
                    MediaCondition = MediaQueryMinWidth(1900),
                    AllowedWidths = new ImmutableArray<int> { 1900, 2400 },
                    Sizes = new ImmutableArray<string> { Size((90, Unit.Vw)) },
                }
            },
        };
    }
}
