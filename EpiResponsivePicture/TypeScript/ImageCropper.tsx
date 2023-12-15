import { AspectRatio } from "./AspectRatio";
import { ResponsiveImageViewModel } from "./ResponsiveImageViewModel";

export class ImageCropper {
  // port from ImageCropper.cs from EpiResponsivePicture
  static getCropSettings(aspectRatio: AspectRatio, image: ResponsiveImageViewModel): string | null {
    if (image == null) return null;

    const focalPoint = image.focalPoint || { x: 0.5, y: 0.5 };

    const sourceWidth = image.width;
    const sourceHeight = image.height;
    const focalPointY = Math.round(sourceHeight * focalPoint.y);
    const focalPointX = Math.round(sourceWidth * focalPoint.x);
    const sourceAspectRatio = sourceWidth / sourceHeight;

    const targetAspectRatio = aspectRatio != null && aspectRatio.ratio > 0 ? aspectRatio.ratio : sourceAspectRatio;

    let x1 = 0;
    let y1 = 0;
    let x2;
    let y2;
    if (targetAspectRatio === sourceAspectRatio) {
      x2 = image.width;
      y2 = image.height;
    } else if (targetAspectRatio > sourceAspectRatio) {
      // the requested aspect ratio is wider than the source image
      const newHeight = Math.floor(sourceWidth / targetAspectRatio);
      x2 = sourceWidth;
      y1 = Math.max(focalPointY - Math.round(newHeight / 2), 0);
      y2 = Math.min(y1 + newHeight, sourceHeight);
      if (y2 === sourceHeight) {
        y1 = y2 - newHeight;
      }
    } else {
      // the requested aspect ratio is narrower than the source image
      const newWidth = Math.round(sourceHeight * targetAspectRatio);
      x1 = Math.max(focalPointX - Math.round(newWidth / 2), 0);
      x2 = Math.min(x1 + newWidth, sourceWidth);
      y2 = sourceHeight;
      if (x2 === sourceWidth) {
        x1 = x2 - newWidth;
      }
    }
    return `${x1},${y1},${x2},${y2}`;
  }
}
