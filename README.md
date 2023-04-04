## How to start

* Install `Forte.EpiResponsivePicture` NuGet package
* Create the Image media type that derives from `Forte.EpiResponsivePicture.ResizedImage.ImageBase` . The base class contains properties required to properly crop images and set Focal point.
* In `Program.cs` (make sure to have `#using Forte.EpiResponsivePicture.Extensions;`)
```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddForteEpiResponsivePicture();

// Removed for brevity

var app = builder.Build();

app.UseForteEpiResponsivePicture();

// Removed for brevity

app.Run();
```
Alternatively if you prefer to use `Startup.cs` file:
```cs
public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;

    public Startup(IWebHostEnvironment webHostingEnvironment)
    {
        _webHostingEnvironment = webHostingEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddForteEpiResponsivePicture();
                
        // Removed for brevity
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseForteEpiResponsivePicture();
        
        // Removed for brevity
    }
}
```

The image type gets "Focal point" property that can be set by editors in All properties view.

If you don't like the inheritance, alternatively you can implement interfaces: `IImage` (for resizing & proper `alt` loading), and `IResponsiveImage` (for focal point and proper cropping).
Remember that `Width` and `Height` properties shouldn't be editable (are automatically set from image when publising). Also add `[BackingType(typeof(PropertyFocalPoint))]` for `FocalPoint` property.

### Support additional URI segments
There are three default URI segments supported by the library:
- `/contentassets`
- `/globalassets`
- `/siteassets`

Use below code if you want to extend that list with custom segments:
```cs
    services.AddForteEpiResponsivePicture(o =>
        new EpiResponsivePicturesOptions
        {
            AdditionalSegments = new [] {"/my-custom-assets-path"}
        });
```
## Render picture element

Package uses `PictureProfile` to describe image sizes in rendered `<picture>` element:
```cs
public static readonly PictureProfile SampleProfile = new PictureProfile
{
    DefaultWidth = 800,
    Sources = new[]
    {
        new PictureSource
        {
            MediaCondition = CssHelpers.MediaQueryMinWidth(1900),
            AllowedWidths = new [] {
                1900,
                2400,
            },
            Sizes = new[]
            {
                CssHelpers.Size((90, CssHelpers.Unit.Vw)),
            },
        },
        new PictureSource
        {
            MediaCondition = CssHelpers.MediaQueryMinWidth(1000),
            AllowedWidths = new []
            {
                1000,
                1200,
                1400,
                1600,
            },
            Mode = ScaleMode.Crop,
            TargetAspectRatio = AspectRatio.Create(16,9),
            Sizes = new []
            {
                CssHelpers.MediaQueryMinWidthWithSize(1400, 1400),
                CssHelpers.Size((100, CssHelpers.Unit.Vw)),
            },
        },
        new PictureSource
        {
            MediaCondition = CssHelpers.MediaQueryMaxWidth(1000),
            AllowedWidths = new [] {
                1000,
                1200,
                1400,
                1600,
            },
            Mode = ScaleMode.Crop,
            TargetAspectRatio = AspectRatio.Create(1),
            Quality = PictureQuality.Create(60),
            Sizes = new [] {
                CssHelpers.Size((50, CssHelpers.Unit.Vw)),
            },
        }
    },
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

**(NOTE: markup examples are valid in case you're using built-in ImageSharp. In other cases names of resize-related parameters, like `width` or `rxy` may vary)** : 
```html
<picture>
    <source media="(min-width: 1900px)" sizes="90vw" srcset="/globalassets/pexels-amar.jpg?width=1900&rxy=0.5,0.5 1900w, /globalassets/pexels-amar.jpg?width=2400&rxy=0.5,0.5 2400w">
    <source media="(min-width: 1000px)" sizes="(min-width: 1400px) 1400px, 100vw" srcset="/globalassets/pexels-amar.jpg?width=1000&height=562&rxy=0.5,0.5&rmode=Crop 1000w, /globalassets/pexels-amar.jpg?width=1200&height=675&rxy=0.5,0.5&rmode=Crop 1200w, /globalassets/pexels-amar.jpg?width=1400&height=788&rxy=0.5,0.5&rmode=Crop 1400w, /globalassets/pexels-amar.jpg?width=1600&height=900&rxy=0.5,0.5&rmode=Crop 1600w">
    <source media="(max-width: 1000px)" sizes="50vw" srcset="/globalassets/pexels-amar.jpg?width=1000&height=1000&quality=60&rxy=0.5,0.5&rmode=Crop 1000w, /globalassets/pexels-amar.jpg?width=1200&height=1200&quality=60&rxy=0.5,0.5&rmode=Crop 1200w, /globalassets/pexels-amar.jpg?width=1400&height=1400&quality=60&rxy=0.5,0.5&rmode=Crop 1400w, /globalassets/pexels-amar.jpg?width=1600&height=1600&quality=60&rxy=0.5,0.5&rmode=Crop 1600w">
    <img alt="" src="/globalassets/pexels-amar.jpg?width=1000&rxy=0.5,0.5"></picture>
```

Should you ever find yourself in need of generating only body of picture tag (ie when creating tag in React) you should make use of
```cs
@Html.ResizedPictureSources(Model, PictureProfiles.SampleProfile)
```

Following will result in:
```html
<source media="(min-width: 1900px)" sizes="90vw" srcset="/globalassets/pexels-amar.jpg?width=1900&rxy=0.5,0.5 1900w, /globalassets/pexels-amar.jpg?width=2400&rxy=0.5,0.5 2400w">
<source media="(min-width: 1000px)" sizes="(min-width: 1400px) 1400px, 100vw" srcset="/globalassets/pexels-amar.jpg?width=1000&height=562&rxy=0.5,0.5&rmode=Crop 1000w, /globalassets/pexels-amar.jpg?width=1200&height=675&rxy=0.5,0.5&rmode=Crop 1200w, /globalassets/pexels-amar.jpg?width=1400&height=788&rxy=0.5,0.5&rmode=Crop 1400w, /globalassets/pexels-amar.jpg?width=1600&height=900&rxy=0.5,0.5&rmode=Crop 1600w">
<source media="(max-width: 1000px)" sizes="50vw" srcset="/globalassets/pexels-amar.jpg?width=1000&height=1000&quality=60&rxy=0.5,0.5&rmode=Crop 1000w, /globalassets/pexels-amar.jpg?width=1200&height=1200&quality=60&rxy=0.5,0.5&rmode=Crop 1200w, /globalassets/pexels-amar.jpg?width=1400&height=1400&quality=60&rxy=0.5,0.5&rmode=Crop 1400w, /globalassets/pexels-amar.jpg?width=1600&height=1600&quality=60&rxy=0.5,0.5&rmode=Crop 1600w">
<img alt="" src="/globalassets/pexels-amar.jpg?width=1000&rxy=0.5,0.5">
```

## CSS Helpers
To avoid writing media-queries as strings a couple of helper methods are provided in `CssHelpers`
* `MediaQueryMaxWidth(100)` -> `"(max-width:100px)"`
* `MediaQueryMaxWidth((100, Unit.Pt))` -> `"(max-width:100pt)"` 
* `MediaQueryMinWidth(100)` -> `"(min-width:100px)"` 
* `MediaQueryMinWidth((100, Unit.Pt))` -> `"(min-width:100pt)"` 
* `MediaQueryMaxWidthWithSize(100, 100)` -> `"(max-width:100px) 100px"` 
* `MediaQueryMaxWidthWithSize((100, Unit.Percent), (100, Unit.Rem))` -> `"(max-width:100%) 100rem"`
* `MediaQueryMinWidthWithSize(100, 100)` -> `"(min-width:100px) 100px"`
* `MediaQueryMinWidthWithSize((100, Unit.Percent), (100, Unit.Rem))` -> `"(min-width:100%) 100rem"`
* `Size(100)` -> `"100px"`

## Image caching
By default cached resized images will be stored as blobs in `App_Data` directory. 
Should you ever be in need of storing cached resized images on `Azure Blob Storage` make use of `AddForteEpiResponsivePicture` overload.
```cs
services.AddForteEpiResponsivePicture(options =>
{
    options.ConnectionString = "{CONNECTION_STRING}";
    options.ContainerName = "{CONTAINER_NAME}";
});
```
For development purposes consider making use of `https://hub.docker.com/_/microsoft-azure-storage-azurite` image

## Own image transformation provider
Should you ever be in need of using different image processor than `ImageSharp`:
* Extend `ResizedUrlGeneratorBase`
* Register your own implementation in DI __before__ calling `AddForteEpiResponsivePicture`
```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<YourCustomUrlGenerator>();
builder.Services.AddForteEpiResponsivePicture();
```

## Custom ISourceTagBuilder and IPictureTagBuilder

If you want to have custom logic of creating `<picture>` or `<source>` elements you can register your own implementations of `ISourceTagBuilderProvider` and/or `IPictureTagBuilderProvider` which can produce your own builders. 

## Backwards compatibility
### ImageResizer

In case you were making use of ImageResizer's implementation of FocalPoint below points will guide you through migration process. The main idea is to read focal points that were saved by ImageResizer and convert them into Forte.ResponsivePicture standard. After running this migration you should be good to turn it off as it may have a slight impact on performance.

1. In `Startup.cs` make sure to call `AddForteEpiResponsivePicture` with properly configured `EpiResponsivePicturesOptions`
```cs
    services.AddForteEpiResponsivePicture(o =>
        new EpiResponsivePicturesOptions
        {
            ImageResizerCompatibilityEnabled = true
        });
```
2. In referencing project create a scheduled job that acquires connection string to underlying database and run on it query provided by `IImageResizerFocalPointConversionSqlProvider`
```cs
[ScheduledPlugIn(DisplayName = "Migrate ImageSharp Focal Points To Forte.ResponsivePicture")]
public class MigrateImageSharpFocalPointsJob : ScheduledJobBase
{
    private readonly IPrincipalAccessor principalAccessor;
    private readonly RepublishContentService republishContentService;
    private readonly FocalPointRestoreService focalPointRestoreService;

    public MigrateImageSharpFocalPointsJob(IPrincipalAccessor principalAccessor, RepublishContentService republishContentService, FocalPointRestoreService focalPointRestoreService)
    {
        this.principalAccessor = principalAccessor;
        this.republishContentService = republishContentService;
        this.focalPointRestoreService = focalPointRestoreService;
    }


    public override string Execute()
    {
        focalPointRestoreService.Restore(); // keep in mind that it might be neccessary to elevate job's permissions to Administrator see https://www.gulla.net/en/blog/scheduled-jobs-in-optimizely-cms-12/
        
        var republishedCount = republishContentService.Republish<Image>(); // you will need to republish images after restoring focal points
        return msg;
    }
}
```
3. Restart solution. Unfortunately due to EPiServers internal mechanisms it is required to restart solution to clear caches and reassign property definition types
