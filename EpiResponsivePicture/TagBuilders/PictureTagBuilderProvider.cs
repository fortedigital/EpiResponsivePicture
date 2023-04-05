using Forte.EpiResponsivePicture.GeneratorProfiles;

namespace Forte.EpiResponsivePicture.TagBuilders;

public class PictureTagBuilderProvider : IPictureTagBuilderProvider
{
    private readonly IResizedUrlGenerator resizedUrlGenerator;
    private readonly ISourceTagBuilderProvider sourceTagBuilderProvider;

    public PictureTagBuilderProvider(IResizedUrlGenerator resizedUrlGenerator, ISourceTagBuilderProvider sourceTagBuilderProvider)
    {
        this.resizedUrlGenerator = resizedUrlGenerator;
        this.sourceTagBuilderProvider = sourceTagBuilderProvider;
    }

    public IPictureTagBuilder Create() => new PictureTagBuilder(resizedUrlGenerator, sourceTagBuilderProvider);
}