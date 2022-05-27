using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Moq;
using NUnit.Framework;
using Forte.EpiResponsivePicture.ResizedImage;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
                {typeof(IUrlResolver), urlResolverMock.Object}
            }));
        }

        [TestCase(ResizedImageFormat.Preserve, ExpectedResult = "/images/foo.jpg?w=500")]
        [TestCase(ResizedImageFormat.Jpg, ExpectedResult = "/images/foo.jpg?w=500&format=jpg")]
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
                    "<picture><source media=\"(min-width:1900px)\" sizes=\"90vw\" srcset=\"/images/foo.jpg?w=1900 1900w, /images/foo.jpg?w=2400 2400w\" /><img alt=\"\" src=\"/images/foo.jpg?w=0\" /></picture>"));
        }

        [Test]
        public void When_PictureProfile_Overrides_ImageFormat_ResizedPicture_Adds_Format_To_QueryString()
        {
            HtmlHelper html = null;

            var markup = html.ResizedPicture(new ContentReference(123), profileOverridingFormat, "/images/foo.jpg")
                .ToString();

            Assert.That(markup,
                Is.EqualTo(
                    "<picture><source media=\"(min-width:1900px)\" sizes=\"90vw\" srcset=\"/images/foo.jpg?format=jpg&amp;w=1900 1900w, /images/foo.jpg?format=jpg&amp;w=2400 2400w\" /><img alt=\"\" src=\"/images/foo.jpg?format=jpg&amp;w=0\" /></picture>"));
        }

        private static readonly PictureProfile profileOverridingFormat = new PictureProfile()
        {
            Format = ResizedImageFormat.Jpg,
            Sources = new[]
            {
                new PictureSource()
                {
                    MediaCondition = "(min-width:1900px)",
                    AllowedWidths = new[] {1900, 2400},
                    Sizes = new[]
                    {
                        "90vw"
                    }
                },
            },
        };

        private static readonly PictureProfile profilePreservingFormat = new PictureProfile()
        {
            Sources = new[]
            {
                new PictureSource()
                {
                    MediaCondition = "(min-width:1900px)",
                    AllowedWidths = new[] {1900, 2400},
                    Sizes = new[]
                    {
                        "90vw"
                    }
                },
            },
        };
    }
}