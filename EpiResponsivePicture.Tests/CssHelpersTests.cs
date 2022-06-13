using NUnit.Framework;
using static Forte.EpiResponsivePicture.ResizedImage.CssHelpers;

namespace Forte.EpiResponsivePicture.Tests
{
    [TestFixture]
    public class CssHelperTests
    {
        [TestCase(0, ExpectedResult = "0px")]
        [TestCase(1, ExpectedResult = "1px")]
        [TestCase(33.3, ExpectedResult = "33.3px")]
        public string Test_BuildSize(double value) => Size(value);

        [TestCase(10, Unit.Cm, ExpectedResult = "10cm")]
        [TestCase(10, Unit.Em, ExpectedResult = "10em")]
        [TestCase(10, Unit.In, ExpectedResult = "10in")]
        [TestCase(10, Unit.Mm, ExpectedResult = "10mm")]
        [TestCase(10, Unit.Pc, ExpectedResult = "10pc")]
        [TestCase(10, Unit.Percent, ExpectedResult = "10%")]
        [TestCase(10, Unit.Pt, ExpectedResult = "10pt")]
        [TestCase(10, Unit.Px, ExpectedResult = "10px")]
        [TestCase(10, Unit.Rem, ExpectedResult = "10rem")]
        [TestCase(10, Unit.Vw, ExpectedResult = "10vw")]
        public string Test_BuildSize(double value, Unit unit) => Size((value, unit));

        [TestCase(0, ExpectedResult = "(max-width: 0px)")]
        [TestCase(1, ExpectedResult = "(max-width: 1px)")]
        [TestCase(10, ExpectedResult = "(max-width: 10px)")]
        [TestCase(33.3, ExpectedResult = "(max-width: 33.3px)")]
        public string Test_MediaQueryMaxWidth_Double(double queryWidthPixel) => MediaQueryMaxWidth(queryWidthPixel);
        
        [TestCase(0, Unit.Vw, ExpectedResult = "(max-width: 0vw)")]
        [TestCase(1, Unit.Vw, ExpectedResult = "(max-width: 1vw)")]
        [TestCase(10, Unit.Vw, ExpectedResult = "(max-width: 10vw)")]
        [TestCase(33.3, Unit.Vw, ExpectedResult = "(max-width: 33.3vw)")]
        public string Test_MediaQueryMaxWidth_Tuple(double queryWidthPixel, Unit unit) => MediaQueryMaxWidth((queryWidthPixel, unit));

        [TestCase(0, ExpectedResult = "(min-width: 0px)")]
        [TestCase(1, ExpectedResult = "(min-width: 1px)")]
        [TestCase(10, ExpectedResult = "(min-width: 10px)")]
        [TestCase(33.3, ExpectedResult = "(min-width: 33.3px)")]
        public string Test_MediaQueryMinWidth_Double(double queryWidthPixel) => MediaQueryMinWidth(queryWidthPixel);
        
        [TestCase(0, Unit.Vw, ExpectedResult = "(min-width: 0vw)")]
        [TestCase(1, Unit.Vw, ExpectedResult = "(min-width: 1vw)")]
        [TestCase(10, Unit.Vw, ExpectedResult = "(min-width: 10vw)")]
        [TestCase(33.3, Unit.Vw, ExpectedResult = "(min-width: 33.3vw)")]
        public string Test_MediaQueryMinWidth_Tuple(double queryWidthPixel, Unit unit) => MediaQueryMinWidth((queryWidthPixel, unit));
        
        [TestCase(0, 0, ExpectedResult = "(max-width: 0px) 0px")]
        [TestCase(1, 1, ExpectedResult = "(max-width: 1px) 1px")]
        [TestCase(10, 10, ExpectedResult = "(max-width: 10px) 10px")]
        [TestCase(33.3, 33.3, ExpectedResult = "(max-width: 33.3px) 33.3px")]
        public string Test_MediaQueryMaxWidthWithSize_Double(double queryWidthPixel, double imageWidthPixel) => MediaQueryMaxWidthWithSize(queryWidthPixel, imageWidthPixel);
        
        [TestCase(0, Unit.Rem, 0, Unit.Vw,ExpectedResult = "(max-width: 0rem) 0vw")]
        [TestCase(1, Unit.Rem, 1, Unit.Vw,ExpectedResult = "(max-width: 1rem) 1vw")]
        [TestCase(10, Unit.Rem, 10, Unit.Vw,ExpectedResult = "(max-width: 10rem) 10vw")]
        [TestCase(33.3, Unit.Rem, 33.3, Unit.Vw,ExpectedResult = "(max-width: 33.3rem) 33.3vw")]
        public string Test_MediaQueryMaxWidthWithSize_Tuple(double queryWidth, Unit queryWidthUnit, double imageWidth, Unit imageWidthUnit) => MediaQueryMaxWidthWithSize((queryWidth, queryWidthUnit), (imageWidth, imageWidthUnit));
        
        [TestCase(0, 0, ExpectedResult = "(min-width: 0px) 0px")]
        [TestCase(1, 1, ExpectedResult = "(min-width: 1px) 1px")]
        [TestCase(10, 10, ExpectedResult = "(min-width: 10px) 10px")]
        [TestCase(33.3, 33.3, ExpectedResult = "(min-width: 33.3px) 33.3px")]
        public string Test_MediaQueryMinWidthWithSize_Double(double queryWidthPixel, double imageWidthPixel) => MediaQueryMinWidthWithSize(queryWidthPixel, imageWidthPixel);
        
        [TestCase(0, Unit.Rem, 0, Unit.Vw,ExpectedResult = "(min-width: 0rem) 0vw")]
        [TestCase(1, Unit.Rem, 1, Unit.Vw,ExpectedResult = "(min-width: 1rem) 1vw")]
        [TestCase(10, Unit.Rem, 10, Unit.Vw,ExpectedResult = "(min-width: 10rem) 10vw")]
        [TestCase(33.3, Unit.Rem, 33.3, Unit.Vw,ExpectedResult = "(min-width: 33.3rem) 33.3vw")]
        public string Test_MediaQueryMinWidthWithSize_Tuple(double queryWidth, Unit queryWidthUnit, double imageWidth, Unit imageWidthUnit) => MediaQueryMinWidthWithSize((queryWidth, queryWidthUnit), (imageWidth, imageWidthUnit));
    }
}