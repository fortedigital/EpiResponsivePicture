import { IFocalPoint } from "./generated";

export interface ResponsiveImageViewModel {
  url: string;
  focalPoint: IFocalPoint;
  width: number;
  height: number;
  alt: string;
}
