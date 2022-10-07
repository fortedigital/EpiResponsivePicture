using System;
using EPiServer.Core;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using Forte.EpiResponsivePicture.Configuration;
using Microsoft.Extensions.Options;

namespace Forte.EpiResponsivePicture.ResizedImage.Property;

[PropertyDefinitionTypePlugIn]
public class PropertyFocalPoint : PropertyString
{

    private EpiResponsivePicturesOptions configuration;

    public PropertyFocalPoint() : this(ServiceLocator.Current.GetInstance<IOptions<EpiResponsivePicturesOptions>>()){ }

    public PropertyFocalPoint(IOptions<EpiResponsivePicturesOptions> options)
    {
        configuration = options.Value;
    }
    
    public override Type PropertyValueType => typeof(FocalPoint);

    public override object Value
    {
        get
        {
            var value = base.Value as string;

            if (string.IsNullOrWhiteSpace(value)) return null;

            return FocalPoint.Parse(value, configuration);
        }

        set
        {
            if (value is FocalPoint focalPoint)
                base.Value = focalPoint.ToString();
            else
                base.Value = value;
        }
    }

    public override object SaveData(PropertyDataCollection properties)
    {
        return String;
    }
}
