# Forte.EpiResponsivePicture

Looking how to migrate from Forte.EpiCommonUtils? See MIGRATING.md

## How to start

* When having image resizer with Episerver properly configured (https://github.com/valdisiljuconoks/ImageResizer.Plugins.EPiServerBlobReader), install `Forte.EpiResponsivePicture` NuGet package
* Create the Image media type that derives from `Forte.EpiResponsivePicture.ResizedImage.ImageBase`. The base class contains properties required to properly crop images and set Focal point. 

The image type gets "Focal point" property that can be set by editors in All properties view.

If you don't like the inheritance, alternatively you can implement interfaces: `IImage` (for resizing & proper `alt` loading), and `IResponsiveImage` (for focal point and proper cropping).
Remember that `Width` and `Height` properties shouldn't be editable (are automatically set from image when publising). Also add `[BackingType(typeof(PropertyFocalPoint))]` for `FocalPoint` property.

## Render picture element

Package uses `PictureProfile` to describe image sizes in rendered `<picture>` element:
```cs
public static readonly PictureProfile SampleProfile = new PictureProfile
{
    DefaultWidth = 800,
    MaxImageDimension = 2500,
    Sources = new []
    {
        new PictureSource
        {
            MediaCondition = "(min-width:1900px)",
            AllowedWidths = new [] {1900,2400},
            Sizes = new []
            {
                "90vw"
            }
        }, 
        new PictureSource
        {
            MediaCondition = "(min-width:1000px)",
            AllowedWidths = new [] {1000,1200, 1400,1600},
            Mode = ScaleMode.Crop,
            TargetAspectRatio = AspectRatio.Create(16,9),
            Sizes = new []
            {
                "(min-width: 1400px) 1400px",
                "100vw"
            }
            
        },
        new PictureSource
        {
            MediaCondition = "(max-width:1000px)",
            AllowedWidths = new [] {1000,1200, 1400,1600},
            Mode = ScaleMode.Crop,
            TargetAspectRatio = AspectRatio.Create(1),
            Quality = 60,
            Sizes = new []
            {
                "50vw"
            }
        }
    }
};
```
Above picture profile describes three different `<source>` elements in markup, that would represent, as following:
* for screens above 1900px, return image in original aspect ratio (not cropped), resized to size matching 90% of viewport width, with two possible widths (1900 and 2400 px)
* for screens between 1000px and 1900px, image should be cropped to match 16:9 aspect ratio, and should be resized to 100% of viewport width, up to 1400px
* for screens smaller than 1000px, return square image for half the screen width, with slightly lower quality (60%) 

Defined given picture profile, you no longer have to bother for exact cropping, resizing etc. Any image gets resized & cropped automatically, based on focal point defined by editor. 

To render the image, use `ResizedPicture` HTML helper extension method defined:

```cs
@Html.ResizedPicture(Model, PictureProfiles.SampleProfile)
```

This will render following markup: 
```html
<picture>
    <source media="(min-width:1900px)" sizes="90vw" srcset="/globalassets/path-to-image.jpg?w=1900 1900w, /globalassets/path-to-image.jpg?w=2400 2400w">
    <source media="(min-width:1000px)" sizes="(min-width: 1400px) 1400px, 100vw" srcset="/globalassets/path-to-image.jpg?mode=crop&w=1000&h=562&crop=0,511,1064,1109 1000w, /globalassets/path-to-image.jpg?mode=crop&w=1200&h=675&crop=0,511,1064,1109 1200w, /globalassets/path-to-image.jpg?mode=crop&w=1400&h=787&crop=0,511,1064,1109 1400w, /globalassets/path-to-image.jpg?mode=crop&w=1600&h=900&crop=0,511,1064,1109 1600w">
    <source media="(max-width:1000px)" sizes="(min-width: 1400px) 1400px, 100vw" srcset="/globalassets/path-to-image.jpg?mode=crop&quality=60&w=1000&h=1000&crop=0,278,1064,1342 1000w, /globalassets/path-to-image.jpg?mode=crop&quality=60&w=1200&h=1200&crop=0,278,1064,1342 1200w, /globalassets/path-to-image.jpg?mode=crop&quality=60&w=1400&h=1400&crop=0,278,1064,1342 1400w, /globalassets/path-to-image.jpg?mode=crop&quality=60&w=1600&h=1600&crop=0,278,1064,1342 1600w">
    <img alt="" src="/globalassets/path-to-image.jpg?w=800">
</picture>
```