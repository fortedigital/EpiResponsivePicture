using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Processors;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Forte.EpiResponsivePicture.ResizedImage.Processors;

public class ResizeWebDownscaleProcessor : IImageWebProcessor
{
    public static string Width => ResizeWebProcessor.Width;
    public static string Height => ResizeWebProcessor.Height;
    public static string Orient => ResizeWebProcessor.Orient;

    public IEnumerable<string> Commands => _processor.Commands;

    private readonly IImageWebProcessor _processor;

    public ResizeWebDownscaleProcessor(IImageWebProcessor processor)
    {
        _processor = processor;
    }

    public bool RequiresTrueColorPixelFormat(CommandCollection commands, CommandParser parser, CultureInfo culture)
        => _processor.RequiresTrueColorPixelFormat(commands, parser, culture);

    public FormattedImage Process(
        FormattedImage image,
        ILogger logger,
        CommandCollection commands,
        CommandParser parser,
        CultureInfo culture)
    {
        var orientation = GetExifOrientation(image, commands, parser, culture);
        var size = ParseSize(orientation, commands, parser, culture);

        //Prevents requested image from being upscaled.
        var width = Math.Min(size.Width, image.Image.Width);
        var height = Math.Min(size.Height, image.Image.Height);
        commands[Width] = width.ToString();
        commands[Height] = height.ToString();

        return _processor.Process(image, logger, commands, parser, culture);
    }

    private static ushort GetExifOrientation(FormattedImage image, CommandCollection commands, CommandParser parser, CultureInfo culture)
    {
        if (commands.Contains(Orient) && !parser.ParseValue<bool>(commands.GetValueOrDefault(Orient), culture))
        {
            return ExifOrientationMode.Unknown;
        }

        image.TryGetExifOrientation(out var orientation);
        return orientation;
    }

    private static Size ParseSize(
        ushort orientation,
        CommandCollection commands,
        CommandParser parser,
        CultureInfo culture)
    {
        // The command parser will reject negative numbers as it clamps values to ranges.
        var width = (int)parser.ParseValue<uint>(commands.GetValueOrDefault(Width), culture);
        var height = (int)parser.ParseValue<uint>(commands.GetValueOrDefault(Height), culture);

        return ExifOrientationUtilities.Transform(new Size(width, height), orientation);
    }
}
