using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    [ModuleDependency(typeof(ShellInitialization))]
    [InitializableModule]
    public class ResponsivePictureModule : IInitializableModule
    {
        private ImagePublishingEventHandler _imagePublishingEventHandler;

        public void Initialize(InitializationEngine context)
        {
            _imagePublishingEventHandler = ServiceLocator.Current.GetInstance<ImagePublishingEventHandler>();
            context.Locate.ContentEvents().PublishingContent += _imagePublishingEventHandler.CalculateDimensions;
        }

        public void Uninitialize(InitializationEngine context)
        {
            context.Locate.ContentEvents().PublishingContent -= _imagePublishingEventHandler.CalculateDimensions;
        }
    }
}
