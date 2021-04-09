# epiresponsivepicture-react

> ResponsivePicture React component for Forte.EpiResponsivePicture package

[![NPM](https://img.shields.io/npm/v/epiresponsivepicture-react.svg)](https://www.npmjs.com/package/epiresponsivepicture-react) [![JavaScript Style Guide](https://img.shields.io/badge/code_style-standard-brightgreen.svg)](https://standardjs.com)

## Install

```bash
npm install --save epiresponsivepicture-react
```

## Usage

```tsx
import React, { Component } from 'react'

import ResponsivePicture from 'epiresponsivepicture-react'
import 'epiresponsivepicture-react/dist/index.css'

model: ResponsiveImageViewModel;
  profile: PictureProfile;
  imgAttributes?: any
class Example extends Component {
  render() {
    return <ResponsivePicture model={imageModel} profile={pictureProfile} />
  }
}
```

## Parameters
ResponsivePicture component accepts numerous parameters:
- `model` (required), having following properties:
  - `url` - base URL of the image,
  - `focalPoint` - focal point of the image, with `x` and `y` properties. Each property should be number in range of 0..1
  - `width` and `height` of the original image,
  - `alt` for the `<img>` tag,
- `profile` (required), being serialized picture profile (camel cased)
- `imgAttributes` - set of additional HTML attributes that should be present in `<img>` tag.

## License

MIT © [Forte Digital](https://github.com/fortedigital)
