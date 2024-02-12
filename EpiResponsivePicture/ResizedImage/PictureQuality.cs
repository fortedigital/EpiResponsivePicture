using System;
using Microsoft.Toolkit.Diagnostics;
using Reinforced.Typings.Attributes;

namespace Forte.EpiResponsivePicture.ResizedImage;

[TsInterface(AutoExportProperties = true, AutoExportMethods = false)]
public class PictureQuality : IEquatable<PictureQuality>
{
    public int Quality { get; }
    
    private PictureQuality(int quality)
    {
        if (quality is not (-1 or > 0 and < 100))
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(quality), "Quality must be greater than 0 and less than 100");
        Quality = quality;
    }

    [TsIgnore]
    public static PictureQuality Default => new(-1);

    [TsIgnore]
    public static PictureQuality Create(int quality) => new(quality);
    
    #region IEquatable implementation

    public bool Equals(PictureQuality other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Quality.Equals(other.Quality);
    }
        
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PictureQuality) obj);
    }
        
    public override int GetHashCode()
    {
        return Quality.GetHashCode();
    }
        
    public static bool operator ==(PictureQuality left, PictureQuality right) => left is not null && left.Equals(right);
    public static bool operator !=(PictureQuality left, PictureQuality right) => !(left == right);

    #endregion

    public override string ToString()
    {
        return $"{Quality:00}";
    }
}
