import * as React from "react";
import { ReactNode } from "react";
import { ResponsiveImageViewModel } from "./ResponsiveImageViewModel";
import { IAspectRatio, ScaleMode, IFocalPoint, IPictureProfile, IPictureSource } from '../src/generated'
export function getResizedImageUrl(
  imageUrl: string,
  width: number | null = null,
  height: number | null = null,
  mode: ScaleMode | null = null,
  quality: number | null = null,
  focalPoint: IFocalPoint | null = null
) {
  const urlParams = [];

  if (width) {
    urlParams.push(`width=${width.toString()}`);
  }

  if (height) {
    urlParams.push(`height=${height.toString()}`);
  }

  if (mode) {
    const modeString = ScaleMode[mode];
    urlParams.push(`rmode=${modeString}`);
  }

  if (quality) {
    urlParams.push(`quality=${quality.toString()}`);
  }

  if (focalPoint) {
    urlParams.push(`rxy=${focalPoint.x.toFixed(3)},${focalPoint.y.toFixed(3)}`);
  }

  return urlParams.length > 0 ? `${imageUrl}?${urlParams.join("&")}` : imageUrl;
}

export interface ResponsivePictureProps {
  model: ResponsiveImageViewModel;
  profile: IPictureProfile & { maxImageDimension: number };
  /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
  imgAttributes?: any;
}

export class ResponsivePicture extends React.Component<ResponsivePictureProps> {
  private static buildResizedImageUrl(
    imageUrl: string,
    width: number,
    mode?: ScaleMode,
    targetAspectRatio?: IAspectRatio,
    quality?: number,
    image?: ResponsiveImageViewModel,
    maxImageDimension?: number
  ) {
    const maxDimension = maxImageDimension || 3200;

    let usedWidth = Math.min(width, maxDimension);
    let height = null;

    if (mode !== ScaleMode.Default && mode !== ScaleMode.Max && image != null) {
      if (targetAspectRatio == null || targetAspectRatio.ratio < 0) {
        throw "Aspect ratio is required when ScaleMode is other than Max";
      }

      height = Math.round(usedWidth / targetAspectRatio.ratio);
      if (height > maxDimension) {
        height = maxDimension;
        usedWidth = height * targetAspectRatio.ratio;
      }
    }

    return getResizedImageUrl(imageUrl, width, height, mode, quality, image?.focalPoint);
  }

  /* eslint-disable-next-line @typescript-eslint/member-ordering */
  constructor(props: ResponsivePictureProps) {
    super(props);

    this.createSourceElement = this.createSourceElement.bind(this);
  }

  componentDidMount() {
    // @ts-ignore
    if (window.objectFitPolyfill) {
      // @ts-ignore
      window.objectFitPolyfill();
    }
  }

  private getDefaultUrl(): string {
    const { sources } = this.props.profile;
    const aspectRatio = sources && sources.length > 0 ? sources[sources.length - 1].targetAspectRatio : undefined;

    if (aspectRatio) {
      return ResponsivePicture.buildResizedImageUrl(
        this.getImageUrl(),
        this.props.profile.defaultWidth,
        3,
        aspectRatio,
        50,
        this.props.model
      );
    }

    return ResponsivePicture.buildResizedImageUrl(
      this.getImageUrl(),
      this.props.profile.defaultWidth,
      undefined,
      undefined,
      50,
      undefined
    );
  }

  private getImageUrl(): string {
    return this.props.model.url || "";
  }

  private buildSources(): ReactNode[] {
    if (!this.props.profile.sources) {
      return [];
    }

    return this.props.profile.sources.map(this.createSourceElement);
  }

  private buildSize(
    width: number,
    mode: ScaleMode,
    targetAspectRatio: IAspectRatio,
    quality: number | undefined
  ): string {
    const url = ResponsivePicture.buildResizedImageUrl(
      this.props.model.url,
      width,
      mode,
      targetAspectRatio,
      quality,
      this.props.model,
      this.props.profile.maxImageDimension
    );

    return `${url} ${width}w`;
  }

  private createSourceElement(source: IPictureSource) {
    const srcSets = source.allowedWidths
      .map(width => this.buildSize(width, source.mode, source.targetAspectRatio, source.quality))
      .join(", ");

    return (
      <source
        key={source.mediaCondition}
        media={source.mediaCondition}
        srcSet={srcSets}
        sizes={source.sizes.join(", ")}
      />
    );
  }

  /* eslint-disable-next-line @typescript-eslint/member-ordering */
  render(): ReactNode {
    const sources = this.buildSources();
    return (
      <picture>
        {sources}
        <img
          alt={this.props.model.alt || ""}
          src={this.getDefaultUrl()}
          data-object-fit="cover"
          data-object-position="center"
          {...this.props.imgAttributes}
        />
      </picture>
    );
  }
}
