using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Esf;

namespace Forte.EpiResponsivePicture.ResizedImage;

public class PictureSource
{
    private string _mediaCondition;
    public string MediaCondition
    {
        get
        {
            return _mediaCondition;
        }
        init
        {
            value = value.Trim();
            if (!value.StartsWith("(") && !value.EndsWith(")"))
            {
                _mediaCondition = $"({value})";
            }else if (!value.StartsWith("("))
            {
                _mediaCondition = $"({value}";
            }
            else if (!value.EndsWith(")"))
            {
                _mediaCondition = $"{value})";
            }
            else
            {
                _mediaCondition = value;
            }
        }
    }

    public IReadOnlyCollection<int> AllowedWidths { get; init; } = new List<int>();
    public IReadOnlyCollection<string> Sizes { get; init; } = new List<string>();
    public ScaleMode Mode { get; init; }
    public AspectRatio TargetAspectRatio { get; init; } = AspectRatio.Default;
    public PictureQuality Quality { get; init; } = PictureQuality.Default;
}
