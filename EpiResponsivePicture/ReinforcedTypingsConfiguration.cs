using Reinforced.Typings.Fluent;
using ConfigurationBuilder = Reinforced.Typings.Fluent.ConfigurationBuilder;

namespace Forte.EpiResponsivePicture;

public static class ReinforcedTypingsConfiguration
{
    public static void Configure(ConfigurationBuilder builder)
    {
        builder.Global(config => config.CamelCaseForProperties().UseModules());
    }
}
