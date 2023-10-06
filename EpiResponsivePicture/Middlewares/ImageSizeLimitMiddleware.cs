using Forte.EpiResponsivePicture.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Forte.EpiResponsivePicture.Middlewares
{
    // Code inspired by SixLabors.ImageSharp.Web.Middleware.ImageSharpMiddleware
    public class ImageSizeLimitMiddleware : IMiddleware
    {
        private readonly IRequestParser _requestParser;
        private readonly IImageProvider[] _providers;
        private readonly HashSet<string> _knownCommands;
        private readonly CommandParser _commandParser;
        private readonly CultureInfo _parserCulture;
        private readonly IOptions<EpiResponsivePicturesOptions> _options;
        private readonly ILogger<ImageSizeLimitMiddleware> _logger;

        public ImageSizeLimitMiddleware(
            IRequestParser requestParser,
            IOptions<ImageSharpMiddlewareOptions> imageSharpOptions,
            IEnumerable<IImageProvider> resolvers,
            IEnumerable<IImageWebProcessor> processors,
            CommandParser commandParser,
            IOptions<EpiResponsivePicturesOptions> responsivePictureOptions,
            ILogger<ImageSizeLimitMiddleware> logger)
        {
            _logger = logger;
            _options = responsivePictureOptions;
            _requestParser = requestParser;
            _commandParser = commandParser;
            _providers = resolvers as IImageProvider[] ?? resolvers.ToArray();
            _parserCulture = imageSharpOptions.Value.UseInvariantParsingCulture
                ? CultureInfo.InvariantCulture
                : CultureInfo.CurrentCulture;
            _knownCommands = new HashSet<string>(processors.SelectMany(p => p.Commands), StringComparer.OrdinalIgnoreCase);
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            var commands = _requestParser.ParseRequestCommands(httpContext);
            var unknownCommands = commands.Where(x => !_knownCommands.Contains(x.Key));
            foreach (var unknownCommand in unknownCommands)
            {
                commands.Remove(unknownCommand);
            }

            // Get the correct provider for the request
            var provider = _providers.FirstOrDefault(x => x.Match(httpContext));
            if (provider?.IsValidRequest(httpContext) != true)
            {
                // Nothing to do. Call the next delegate/middleware in the pipeline.
                await next.Invoke(httpContext);
                return;
            }

            var imageCommandContext = new ImageCommandContext(httpContext, commands, _commandParser, _parserCulture);
            var imageWidth = GetPictureSize(imageCommandContext, ImageDimension.Width);
            var imageHeight = GetPictureSize(imageCommandContext, ImageDimension.Height);

            var maxPictureSize = _options.Value.MaxPictureSize ?? int.MaxValue;
            if (imageWidth > maxPictureSize || imageHeight > maxPictureSize)
            {
                // Requested image is too big. Return Error.
                _logger.LogWarning("Requested image of size {imageWidth}x{imageHeight} is too big. Max image size is {maxPictureSize}x{maxPictureSize}.", imageWidth, imageHeight, maxPictureSize, maxPictureSize);
                await ReturnBadRequest(httpContext, $"Requested image of size {imageWidth}x{imageHeight} is too big. Max image size is {maxPictureSize}x{maxPictureSize}.");
                return;
            }

            // Everything is OK. Proceed.
            await next.Invoke(httpContext);
        }

        private enum ImageDimension
        {
            Width,
            Height,
        }

        private static int GetPictureSize(ImageCommandContext context, ImageDimension dimension)
        {
            var key = dimension switch
            {
                ImageDimension.Width => "width",
                _ => "height",
            };

            return context.Commands.Contains(key)
                ? GetNumber(context.Commands[key])
                : 0;
        }

        private static int GetNumber(string input)
        {
            return int.Parse(new string(input.Where(c => char.IsDigit(c)).ToArray()));
        }

        private static Task ReturnBadRequest(HttpContext httpContext, string message)
        {
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return httpContext.Response.WriteAsJsonAsync(new
            {
                Message = message,
            });
        }
    }
}
