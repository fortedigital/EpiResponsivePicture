using System;
using Forte.EpiResponsivePicture.ResizedImage.Property;

namespace Forte.EpiResponsivePicture.ResizedImage;

internal class ImageCropper
{
    public static CropSettings GetCropSettings(AspectRatio aspectRatio, IResponsiveImage image)
    {
        if (image == null)
            return null;

        // must always add crop parameter because of
        // https://github.com/imazen/resizer/issues/247
        var focalPoint = image.FocalPoint ?? FocalPoint.Center;

        var sourceWidth = image.Width;
        var sourceHeight = image.Height;
        var focalPointY = (int) Math.Round(sourceHeight * focalPoint.Y);
        var focalPointX = (int) Math.Round(sourceWidth * focalPoint.X);
        var sourceAspectRatio = (double) sourceWidth / sourceHeight;

        var targetAspectRatio = aspectRatio != null && aspectRatio.Ratio > 0
            ? aspectRatio.Ratio
            : sourceAspectRatio;

        var x1 = 0;
        var y1 = 0;
        int x2;
        int y2;
        if (targetAspectRatio.Equals(sourceAspectRatio))
        {
            x2 = image.Width;
            y2 = image.Height;
        }
        else if (targetAspectRatio > sourceAspectRatio)
        {
            // the requested aspect ratio is wider than the source image
            var newHeight = (int) Math.Floor(sourceWidth / targetAspectRatio);
            x2 = sourceWidth;
            y1 = Math.Max(focalPointY - (int) Math.Round((double) newHeight / 2), 0);
            y2 = Math.Min(y1 + newHeight, sourceHeight);
            if (y2 == sourceHeight) y1 = y2 - newHeight;
        }
        else
        {
            // the requested aspect ratio is narrower than the source image
            var newWidth = (int) Math.Round(sourceHeight * targetAspectRatio);
            x1 = Math.Max(focalPointX - (int) Math.Round((double) newWidth / 2), 0);
            x2 = Math.Min(x1 + newWidth, sourceWidth);
            y2 = sourceHeight;
            if (x2 == sourceWidth) x1 = x2 - newWidth;
        }

        return new CropSettings {X1 = x1, X2 = x2, Y1 = y1, Y2 = y2};
    }
}
