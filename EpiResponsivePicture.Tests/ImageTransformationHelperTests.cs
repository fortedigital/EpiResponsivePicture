using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Moq;
using NUnit.Framework;
using Forte.EpiResponsivePicture.ResizedImage;

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

            ServiceLocator.SetLocator(new DummyServiceLocator(new Dictionary<Type, object>
            {
                {typeof(IContentLoader), contentLoaderMock.Object},
                {typeof(IUrlResolver), urlResolverMock.Object}
            }));
        }

        [Test]
        public void TestResizedImageUrl()
        {
            HtmlHelper html = null;

            var urlWithoutFormat = html.ResizedImageUrl(new ContentReference(123), 500).ToString();
            var urlWithFormat = html.ResizedImageUrl(new ContentReference(123), 500, ResizedImageFormat.Jpg).ToString();

            Assert.That(urlWithoutFormat, Is.EqualTo("/images/foo.jpg?w=500"));
            Assert.That(urlWithFormat, Is.EqualTo("/images/foo.jpg?w=500&format=jpg"));
        }

        [Test]
        public void TestResizedPicture()
        {
            HtmlHelper html = null;

            var profileWithoutFormat = new PictureProfile()
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

            var profileWithFormat = new PictureProfile()
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
            
            var urlWithoutFormat = html.ResizedPicture(new ContentReference(123), profileWithoutFormat, "/images/foo.jpg").ToString();
            var urlWithFormat = html.ResizedPicture(new ContentReference(123), profileWithFormat, "/images/foo.jpg").ToString();

            Assert.That(urlWithoutFormat, Is.EqualTo("<picture><source media=\"(min-width:1900px)\" sizes=\"90vw\" srcset=\"/images/foo.jpg?w=1900 1900w, /images/foo.jpg?w=2400 2400w\" /><img alt=\"\" src=\"/images/foo.jpg?w=0\" /></picture>"));
            Assert.That(urlWithFormat, Is.EqualTo("<picture><source media=\"(min-width:1900px)\" sizes=\"90vw\" srcset=\"/images/foo.jpg?format=jpg&amp;w=1900 1900w, /images/foo.jpg?format=jpg&amp;w=2400 2400w\" /><img alt=\"\" src=\"/images/foo.jpg?format=jpg&amp;w=0\" /></picture>"));
        }
    }

    public class DummyServiceLocator : IServiceLocator
    {
        private readonly IReadOnlyDictionary<Type, object> services;

        public DummyServiceLocator(IReadOnlyDictionary<Type, object> services)
        {
            this.services = services;
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType)
        {
            return this.services[serviceType];
        }

        public TService GetInstance<TService>()
        {
            return (TService) this.GetInstance(typeof(TService));
        }

        public bool TryGetExistingInstance(Type serviceType, out object instance)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}