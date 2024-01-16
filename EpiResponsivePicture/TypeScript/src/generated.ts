//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

export interface IAspectRatio
{
	ratio: number;
	hasValue: boolean;
}
export interface ICropSettings
{
	x1: number;
	y1: number;
	x2: number;
	y2: number;
	ToString() : string;
}
export interface IPictureProfile
{
	defaultWidth: number;
	format: ResizedImageFormat;
	maxImageDimension: number;
	sources: IPictureSource[];
}
export interface IPictureSource
{
	mediaCondition: string;
	allowedWidths: number[];
	sizes: string[];
	mode: ScaleMode;
	targetAspectRatio: IAspectRatio;
	quality: any;
}
export enum ResizedImageFormat {
	Preserve = 0,
	Jpg = 1,
	Jpeg = 1,
	Png = 2,
	Gif = 3,
	Bmp = 4
}
export enum ScaleMode {
	Default = 0,
	Crop = 1,
	Pad = 2,
	BoxPad = 3,
	Max = 4,
	Min = 5,
	Stretch = 6
}
export interface IFocalPoint
{
	x: number;
	y: number;
	center: IFocalPoint;
}
