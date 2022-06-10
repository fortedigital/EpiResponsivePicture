namespace Forte.EpiResponsivePicture.ResizedImage
{
    public static class CssHelpers
    {
        public enum Unit
        {
            Px = 0,
            Rem,
            Em,
            Pt,
            In,
            Cm,
            Mm,
            Pc,
            Vw,
            Percent,
        }

        internal static string AsString(this Unit unit) => unit switch
        {
            Unit.Percent => "%",
            _ => unit.ToString().ToLowerInvariant(),
        };
    }
}