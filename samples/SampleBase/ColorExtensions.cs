using SixLabors.ImageSharp.PixelFormats;
namespace SampleBase
{
    public static class ColorExtensions
    {
        public static System.Drawing.Color ToSystemDrawingColor(this SixLabors.ImageSharp.Color c){
            var converted = c.ToPixel<Argb32>();
            return System.Drawing.Color.FromArgb((int)converted.Argb);
        }
        public static SixLabors.ImageSharp.Color ToImageSharpColor(this System.Drawing.Color c){
            return SixLabors.ImageSharp.Color.FromRgba(c.R,c.G,c.B,c.A);
        }
    }
}