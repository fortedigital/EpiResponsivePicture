using System.Globalization;
using System.Linq;

namespace Forte.EpiResponsivePicture.ResizedImage.Property
{
    public class FocalPoint
    {
        public FocalPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public static FocalPoint Center => new(0.5, 0.5);

        public override string ToString()
        {
            return $"{X:0.###}|{Y:0.###}";
        }

        public static implicit operator FocalPoint(string s) => Parse(s);
        
        public static FocalPoint Parse(string input)
        {
            var parsed = 
                input.Split('|')
                    .Select(s => s.Replace(',', '.'))
                    .Select(s => double.Parse(s, CultureInfo.InvariantCulture))
                    .ToList();

            return new FocalPoint(parsed[0], parsed[1]);
        }
    }
}
