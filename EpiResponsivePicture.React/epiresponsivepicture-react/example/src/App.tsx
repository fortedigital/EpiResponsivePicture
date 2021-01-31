import React from 'react'
import { ResponsiveImageViewModel, ResponsivePicture, PictureProfile, ScaleMode } from 'epiresponsivepicture-react'
import 'epiresponsivepicture-react/dist/index.css'

const model: ResponsiveImageViewModel = {
  url: "http://resizer.apphb.com/fountain-small.jpg",
  alt: "Fountain",
  focalPoint: { x: 0.1, y: 0.2},
  width: 1496,
  height: 617
};

export interface FocalPoint {
  x: number;
  y: number;
}

const profile: PictureProfile = {
  defaultWidth: 800,
  sources: [
    {
      allowedWidths: [300, 600, 800, 1000, 1200],
      mode: ScaleMode.Crop,
      sizes: [
        "30vw"
      ],
      targetAspectRatio: {ratio: 16/9, hasValue: true}
    }
  ]
}

const App = () => {
  return <ResponsivePicture model={model} profile={profile} />
}

export default App
