using Forte.EpiResponsivePicture.GeneratorProfiles;

namespace Forte.EpiResponsivePicture.TagBuilders;

public class SourceTagBuilderProvider : ISourceTagBuilderProvider
{
    private readonly IResizedUrlGenerator resizedUrlGenerator;

    public SourceTagBuilderProvider(IResizedUrlGenerator resizedUrlGenerator)
    {
        this.resizedUrlGenerator = resizedUrlGenerator;
    }

    public ISourceTagBuilder Create() => new SourceTagBuilder(resizedUrlGenerator);
}