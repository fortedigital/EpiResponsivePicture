import { FocalPoint } from "./FocalPoint";

export interface ResponsiveImageViewModel {
  url: string;
  focalPoint: FocalPoint;
  width: number;
  height: number;
  alt: string;
}
