using System.Drawing;
using MathNet.Numerics.LinearAlgebra.Single;
using System.Reflection;
using GraphSharp.GraphDrawer;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace SampleBase
{
    public class ImageSharpShapeDrawer : IShapeDrawer
    {
        public IImageProcessingContext Context { get; }
        public double FontSize { get; }
        public SixLabors.ImageSharp.Image<Rgba32> Image { get; }
        public Font Font { get; }
        public ImageSharpShapeDrawer(IImageProcessingContext context, SixLabors.ImageSharp.Image<Rgba32> image, double fontSize)
        {
            this.Context = context;
            this.FontSize = fontSize;
            Image = image;
            FontCollection fonts = new FontCollection();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "SampleBase.NotoSans-Bold.ttf";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName) ?? Stream.Null)
            {
                fonts.Install(stream);
            }

            var notoSans = fonts.CreateFont("Noto Sans", ((float)fontSize) * image.Height, FontStyle.Regular);

            Font = notoSans;
        }

        public void Clear(Color color)
        {
            Context.Clear(new SixLabors.ImageSharp.GraphicsOptions(), new SolidBrush(color.ToImageSharpColor()));
        }

        public void DrawLine(Vector start, Vector end, Color color, double thickness)
        {
            var brush = new SolidBrush(color.ToImageSharpColor());
            Context.DrawLines(new DrawingOptions() { }, brush, ((float)thickness), new(start[0],start[1]), new(end[0],end[1]));
        }
        public void DrawText(string text, Vector position, Color color,double fontSize = -1)
        {
            Context.DrawText(text, Font, color.ToImageSharpColor(), new(position[0],position[1]));
        }

        public void FillEllipse(Vector position, double width, double height, Color color)
        {
            var ellipse = new EllipsePolygon(new(position[0],position[1]), ((float)(width+height))/2);
            var brush = new SolidBrush(color.ToImageSharpColor());
            Context.FillPolygon(new DrawingOptions() { }, brush, ellipse.Points.ToArray());
        }
    }
}