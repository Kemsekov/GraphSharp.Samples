using System.Drawing;
using System.Numerics;
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

        public void DrawLine(Vector2 start, Vector2 end, Color color, double thickness)
        {
            var brush = new SolidBrush(color.ToImageSharpColor());
            Context.DrawLines(new DrawingOptions() { }, brush, ((float)thickness), start, end);
        }
        public void DrawText(string text, Vector2 position, Color color,double fontSize = -1)
        {
            Context.DrawText(text, Font, color.ToImageSharpColor(), position);
        }

        public void FillEllipse(Vector2 position, double width, double height, Color color)
        {
            var ellipse = new EllipsePolygon(position, ((float)(width+height))/2);
            var brush = new SolidBrush(color.ToImageSharpColor());
            Context.FillPolygon(new DrawingOptions() { }, brush, ellipse.Points.ToArray());
        }
    }
}