using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace Forte.EpiResponsivePicture.Infrastructure;

[InitializableModule]
[ModuleDependency(typeof(ShellInitialization))]
public class PropertyNamespaceFixModule : IInitializableModule
{
    public void Initialize(InitializationEngine context)
    {
        var repository = context.Locate.Advanced.GetInstance<IPropertyDefinitionTypeRepository>();

        var responsivePictureType = repository
            .List()
            .FirstOrDefault(x => x.IsSystemType() == false && x.AssemblyName?.StartsWith("EpiResponsivePicture") == true);

        if (responsivePictureType != null)
        {
            var definitionClone = responsivePictureType.CreateWritableClone();
            definitionClone.AssemblyName = "Forte.EpiResponsivePicture";
            repository.Save(definitionClone);
        }
    }

    public void Uninitialize(InitializationEngine context)
    {
    }
}