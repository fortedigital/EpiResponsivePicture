using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Forte.EpiResponsivePicture.ResizedImage.Property
{
    [EditorDescriptorRegistration(
        EditorDescriptorBehavior = EditorDescriptorBehavior.Default,
        TargetType = typeof(FocalPoint))]
    public class FocalPointEditorDescriptor : StringEditorDescriptor
    {
        public FocalPointEditorDescriptor()
        {
            ClientEditingClass = "imagepointeditor/imagepointproperty";
        }
    }
}