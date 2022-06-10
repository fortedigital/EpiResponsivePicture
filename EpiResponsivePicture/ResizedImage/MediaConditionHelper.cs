using System.Globalization;
using static Forte.EpiResponsivePicture.ResizedImage.CssHelpers;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public static class MediaQueryHelper
    {
        private const string Max = "max";
        private const string Min = "min";

        public static string MaxWidth(double queryWidth, double imageWidth, Unit queryWidthUnit = Unit.Px,
            Unit imageWidthUnit = Unit.Px) =>
            BuildMediaQueryString(Max, queryWidth, imageWidth, queryWidthUnit, imageWidthUnit);
        
        public static string MinWidth(double queryWidth, double imageWidth, Unit queryWidthUnit = Unit.Px,
            Unit imageWidthUnit = Unit.Px) => 
            BuildMediaQueryString(Min, queryWidth, imageWidth, queryWidthUnit, imageWidthUnit);
        
        private static string BuildMediaQueryString(string mediaQueryType, double queryWidth, double imageWidth,
            Unit queryWidthUnit, Unit imageWidthUnit) => 
            $"({mediaQueryType}-width: {queryWidth.ToString(CultureInfo.InvariantCulture)}{queryWidthUnit.AsString()}) {imageWidth.ToString(CultureInfo.InvariantCulture)}{imageWidthUnit.AsString()}";
    }
}