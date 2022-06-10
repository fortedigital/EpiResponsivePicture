using System;
using System.Globalization;
using EPiServer.Events;
using static Forte.EpiResponsivePicture.ResizedImage.CssHelpers;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public static class CssHelper
    {
        private const string Max = "max";
        private const string Min = "min";

        public static string MediaQueryMaxWidth(double queryWidthPixel) => BuildMediaQueryString(Max, (queryWidthPixel, Unit.Px));
        
        public static string MediaQueryMaxWidth(ValueTuple<double, Unit> queryWidth) => BuildMediaQueryString(Max, queryWidth);
        
        public static string MediaQueryMaxWidthWithSize(double queryWidthPixel, double imageWidthPixel) => 
            BuildMediaQueryWithSizeString(Max, (queryWidthPixel, Unit.Px), (imageWidthPixel, Unit.Px));

        public static string MediaQueryMaxWidthWithSize(ValueTuple<double, Unit> queryWidth, ValueTuple<double, Unit> imageWidth)
            => BuildMediaQueryWithSizeString(Max, queryWidth, imageWidth);
        
        public static string MediaQueryMinWidth(double queryWidthPixel) => BuildMediaQueryString(Min, (queryWidthPixel, Unit.Px));
        
        public static string MediaQueryMinWidth(ValueTuple<double, Unit> queryWidth) => BuildMediaQueryString(Min, queryWidth);
        
        public static string MediaQueryMinWidthWithSize(double queryWidthPixel, double imageWidthPixel) => 
            BuildMediaQueryWithSizeString(Min, (queryWidthPixel, Unit.Px), (imageWidthPixel, Unit.Px));

        public static string MediaQueryMinWidthWithSize(ValueTuple<double, Unit> queryWidth, ValueTuple<double, Unit> width)
            => BuildMediaQueryWithSizeString(Min, queryWidth, width);

        public static string SizePixel(double widthPixel) => BuildSize((widthPixel, Unit.Px));
        public static string Size(ValueTuple<double, Unit> width) => BuildSize(width);
        
        private static string BuildMediaQueryWithSizeString(string mediaQueryType, ValueTuple<double, Unit> queryWidth, ValueTuple<double, Unit> width) => 
            $"{BuildMediaQueryString(mediaQueryType, queryWidth)} {BuildSize(width)}";

        private static string BuildMediaQueryString(string mediaQueryType, ValueTuple<double, Unit> queryWidth) =>
            $"({mediaQueryType}-width: {queryWidth.Item1.ToString(CultureInfo.InvariantCulture)}{queryWidth.Item2.AsString()})";

        private static string BuildSize(ValueTuple<double, Unit> width) => $"{width.Item1.ToString(CultureInfo.InvariantCulture)}{width.Item2.AsString()}";
    }
}