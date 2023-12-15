using System;
using Microsoft.Toolkit.Diagnostics;
using Reinforced.Typings.Attributes;

namespace Forte.EpiResponsivePicture.ResizedImage;

[TsInterface(AutoExportProperties = true, AutoExportMethods = false)]
public class AspectRatio : IEquatable<AspectRatio>
{
    private AspectRatio(double ratio)
    {
        if (ratio is not (-1 or > 0))
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(ratio), "Ratio must be greater than 0");
        Ratio = ratio;
    }

    public static AspectRatio Original => new(-1);

    public static AspectRatio Default => Original;

    public double Ratio { get; }
    public bool HasValue => Ratio > 0;

    public static AspectRatio Create(double width, double height)
    {
        return new(width / height);
    }

    public static AspectRatio Create(double ratio)
    {
        return new(ratio);
    }
        
    #region IEquatable implementation

    public bool Equals(AspectRatio other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Ratio.Equals(other.Ratio);
    }
        
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AspectRatio) obj);
    }
        
    public override int GetHashCode()
    {
        return Ratio.GetHashCode();
    }
        
    public static bool operator ==(AspectRatio left, AspectRatio right) => left is not null && left.Equals(right);
    public static bool operator !=(AspectRatio left, AspectRatio right) => !(left == right);

    #endregion
}
