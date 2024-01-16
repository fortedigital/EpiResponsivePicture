using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Forte.EpiResponsivePicture.ResizedImage.Property;

[EditorDescriptorRegistration(
    EditorDescriptorBehavior = EditorDescriptorBehavior.Default,
    TargetType = typeof(FocalPoint))]
public class FocalPointEditorDescriptor : EditorDescriptor
{
    public const string UIHint = nameof(FocalPointEditorDescriptor);
    
    public FocalPointEditorDescriptor()
    {
        ClientEditingClass = "imagepointeditor/imagepointproperty";
    }
}