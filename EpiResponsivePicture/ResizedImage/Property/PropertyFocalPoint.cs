using System;
using EPiServer.Core;
using EPiServer.PlugIn;

namespace Forte.EpiResponsivePicture.ResizedImage.Property
{
    [PropertyDefinitionTypePlugIn]
    public class PropertyFocalPoint : PropertyString
    {
        public override Type PropertyValueType => typeof(FocalPoint);

        public override object Value
        {
            get
            {
                var value = base.Value as string;

                if (string.IsNullOrWhiteSpace(value)) return null;

                return FocalPoint.Parse(value);
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
}