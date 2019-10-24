namespace Forte.EpiResponsivePicture.ResizedImage
{
    public class AspectRatio
    {
        private AspectRatio(double ratio)
        {
            Ratio = ratio;
        }

        public static AspectRatio Original => new AspectRatio(-1);

        public double Ratio { get; }
        public bool HasValue => Ratio > 0;

        public static AspectRatio Create(double width, double height)
        {
            return new AspectRatio(width / height);
        }

        public static AspectRatio Create(double ratio)
        {
            return new AspectRatio(ratio);
        }
    }
}