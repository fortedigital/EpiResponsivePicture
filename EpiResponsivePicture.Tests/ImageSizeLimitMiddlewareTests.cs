using Forte.EpiResponsivePicture.Configuration;
using Forte.EpiResponsivePicture.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Commands.Converters;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;
using System.Threading.Tasks;

namespace Forte.EpiResponsivePicture.Tests
{
    [TestFixture]
    internal class ImageSizeLimitMiddlewareTests
    {
        private Mock<IRequestParser> _requestParser;
        private Mock<IImageProvider> _resolver;
        private Mock<IImageWebProcessor> _processor;
        private Mock<ICommandConverter> _converter;
        private Mock<ILogger<ImageSizeLimitMiddleware>> _logger;

        private CommandParser _commandParser;
        private IOptions<EpiResponsivePicturesOptions> _responsivePictureOptions;
        private IOptions<ImageSharpMiddlewareOptions> _imageSharpOptions;

        private ImageSizeLimitMiddleware _middleware;

        [SetUp]
        public void Setup()
        {
            _logger = new();
            _resolver = new();
            _processor = new();
            _converter = new();
            _requestParser = new();

            var commands = new CommandCollection
            {
                { "width", "640" },
                { "height", "480" }
            };
            _requestParser
                .Setup(x => x.ParseRequestCommands(It.IsAny<HttpContext>()))
                .Returns(commands);
            _processor
                .Setup(x => x.Commands)
                .Returns(new string[] { "width", "height" });
            _resolver
                .SetupGet(x => x.Match)
                .Returns((HttpContext context) => true);

            var resolvers = new IImageProvider[] { _resolver.Object };
            var processors = new IImageWebProcessor[] { _processor.Object };
            var converters = new ICommandConverter[] { _converter.Object };
            _commandParser = new(converters);
            _imageSharpOptions = Options.Create(new ImageSharpMiddlewareOptions());
            _responsivePictureOptions = Options.Create(new EpiResponsivePicturesOptions());

            _middleware = new(
                _requestParser.Object,
                _imageSharpOptions,
                resolvers,
                processors,
                _commandParser,
                _responsivePictureOptions,
                _logger.Object);
        }

        [Test]
        public async Task When_Request_Is_Not_About_Picture_Proceed()
        {
            var isNextCalled = false;
            var context = new DefaultHttpContext();
            Task next(HttpContext context)
            {
                isNextCalled = true;
                return Task.CompletedTask;
            }
            _resolver
                .Setup(x => x.IsValidRequest(context))
                .Returns(false);

            await _middleware.InvokeAsync(context, next);

            Assert.AreEqual(true, isNextCalled);
        }

        [Test]
        public async Task When_Request_Is_About_Properly_Sized_Picture_Proceed()
        {
            var isNextCalled = false;
            var context = new DefaultHttpContext();
            Task next(HttpContext context)
            {
                isNextCalled = true;
                return Task.CompletedTask;
            }
            _responsivePictureOptions.Value.MaxPictureSize = 1000;
            _resolver
                .Setup(x => x.IsValidRequest(context))
                .Returns(true);

            await _middleware.InvokeAsync(context, next);

            Assert.AreEqual(true, isNextCalled);
        }

        [Test]
        public async Task When_Request_Is_About_Improperly_Sized_Picture_Return_BadRequest()
        {
            var isNextCalled = false;
            var context = new DefaultHttpContext();
            Task next(HttpContext context)
            {
                isNextCalled = true;
                return Task.CompletedTask;
            }
            _responsivePictureOptions.Value.MaxPictureSize = 500;
            _resolver
                .Setup(x => x.IsValidRequest(context))
                .Returns(true);

            await _middleware.InvokeAsync(context, next);

            Assert.AreEqual(false, isNextCalled);
            Assert.AreEqual(true, context.Response.StatusCode == 400);
        }
    }
}
