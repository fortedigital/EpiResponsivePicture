import * as React from "react";
import {ReactNode} from "react";
import {ImageCropper} from "./imageCropper";

export interface FocalPoint
{
	x: number;
	y: number;
}

export interface AspectRatio
{
	ratio: number;
	hasValue: boolean;
}

export interface CropSettings
{
	x1: number;
	y1: number;
	x2: number;
	y2: number;
}

export interface PictureProfile
{
	defaultWidth: number;
	maxImageDimension?: number;
	sources: PictureSource[];
}

export interface PictureSource
{
	mediaCondition?: string;
	allowedWidths: number[];
	sizes: string[];
	mode: ScaleMode;
	targetAspectRatio: AspectRatio;
	quality?: number;
}

export enum ScaleMode {
	Default = 0,
	Max = 1,
	Pad = 2,
	Crop = 3,
	Carve = 4,
	Stretch = 5
}

export interface ResponsiveImageViewModel
{
  url: string;
	focalPoint: FocalPoint;
	width: number;
  height: number;
	alt: string;
}

export interface ResponsivePictureProps
{
  model: ResponsiveImageViewModel;
  profile: PictureProfile;
  imgAttributes?: any
}

export class ResponsivePicture extends React.Component<ResponsivePictureProps> {
    constructor(props: ResponsivePictureProps) {
        super(props);

        this.createSourceElement = this.createSourceElement.bind(this);
    }

    public render(): ReactNode {
        const sources = this.buildSources();
        return (
            <picture>
                {sources}
                <img alt={this.props.model.alt || ""} src={this.getDefaultUrl()} data-object-fit="cover" data-object-position="center" {...this.props.imgAttributes} />
            </picture>);
    }

    public componentDidMount() {
        // @ts-ignore
        if(window.objectFitPolyfill) {
            // @ts-ignore
            window.objectFitPolyfill();
        }
    }

    private buildSources(): ReactNode[] {
        if(!this.props.profile.sources) {
            return [];
        }

        return this.props.profile.sources.map(this.createSourceElement)
    }

    private createSourceElement(source: PictureSource) {

        const srcSets = source.allowedWidths
            .map(width => this.buildSize(width, source.mode, source.targetAspectRatio, source.quality))
            .join(", ");

        return <source key={source.mediaCondition}
                       media={source.mediaCondition}
                       srcSet={srcSets}
                       sizes={source.sizes.join(", ")}/>;
    }

    private buildSize(width: number, mode: ScaleMode, targetAspectRatio: AspectRatio, quality: number | undefined): string {
        const url = this.buildResizedImageUrl(this.props.model.url, width, mode, targetAspectRatio, quality,
            this.props.model,
            this.props.profile.maxImageDimension);

        return `${url} ${width}w`;
    }

    private buildResizedImageUrl(imageUrl: string, width: number,
    mode?: ScaleMode, targetAspectRatio?: AspectRatio, quality?: number,
    image?: ResponsiveImageViewModel, maxImageDimension?: number) {

        maxImageDimension = maxImageDimension || 3200;

        width = Math.min(width, maxImageDimension);
        let height = null;
        let crop = null;

        if (mode !== ScaleMode.Default && mode !== ScaleMode.Max
            && image != null) {
            if (targetAspectRatio == null || targetAspectRatio.ratio < 0) {
                throw "Aspect ratio is required when ScaleMode is other than Max";
            }

            height = Math.round(width / targetAspectRatio.ratio);
            if (height > maxImageDimension) {
                height = maxImageDimension;
                width = (height * targetAspectRatio.ratio);
            }

            crop = ImageCropper.getCropSettings(targetAspectRatio, image);
        }

        return getResizedImageUrl(
            imageUrl,
            width,
            height,
            mode,
            quality,
            crop);
    }

    private getDefaultUrl(): string {
        let sources = this.props.profile.sources;
        let aspectRatio = sources && sources.length > 0 ? sources[sources.length - 1].targetAspectRatio : undefined;

        if (aspectRatio) {
            return this.buildResizedImageUrl(this.getImageUrl(), this.props.profile.defaultWidth, 3, aspectRatio, 50, this.props.model);
        } else {
            return this.buildResizedImageUrl(this.getImageUrl(), this.props.profile.defaultWidth, undefined, undefined, 50, undefined);
        }
    }

    private getImageUrl():string {
        return this.props.model.url || "";
    }
}

export function getResizedImageUrl(imageUrl: string, width: number | null = null, height: number | null = null, mode : ScaleMode | null = null, quality : number | null = null, crop: string | null = null) {
    const urlParams = [];

    if(width){
        urlParams.push(`w=${width.toString()}`);
    }

    if(height){
        urlParams.push(`h=${height.toString()}`);
    }

    if(mode) {
        const modeString = ScaleMode[mode];
        urlParams.push(`mode=${modeString}`);
    }

    if(quality) {
        urlParams.push(`quality=${quality.toString()}`);
    }

    if(crop){
        urlParams.push(`crop=${crop}`);
    }

    return urlParams.length > 0 ? imageUrl + "?" + urlParams.join("&"):imageUrl;
}
