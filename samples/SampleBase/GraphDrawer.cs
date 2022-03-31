
// using System.Drawing;
using GraphSharp.Nodes;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Fonts;
using System.Reflection;

public class GraphDrawer
{
    public float Thickness;
    public float NodeSize;
    public Image<Rgba32> Image;

    public Font Font;

    public GraphDrawer(Image<Rgba32> image, float fontSize)
    {
        Image = image;
        FontCollection fonts = new FontCollection();

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "SampleBase.NotoSans-Bold.ttf";
        using (Stream stream = assembly.GetManifestResourceStream(resourceName) ?? Stream.Null)
        {
            fonts.Install(stream);
        }

        var notoSans = fonts.CreateFont("Noto Sans", fontSize * image.Height, FontStyle.Regular);

        Font = notoSans;
    }
    public Image<Rgba32> Clear(Color color)
    {
        Image.Mutate(x => x.Clear(new GraphicsOptions(), new SolidBrush(color)));
        return Image;
    }
    public Image<Rgba32> DrawNodes(IList<NodeXY> nodes)
    {
        Image.Mutate(x =>
        {
            var size = x.GetCurrentSize();

            Parallel.ForEach(nodes, node =>
                DrawNode(x, node, size, NodeSize));
            Parallel.ForEach(nodes, node =>
                DrawNodeId(x, node, size));
        });
        return Image;
    }
    public Image<Rgba32> DrawEdges(IList<NodeXY> nodes)
    {
        Image.Mutate(x =>
        {
            var size = x.GetCurrentSize();
            Parallel.ForEach(nodes, node =>
            {
                foreach (var c in node.Edges)
                {
                    DrawEdge(x, c, size);
                }
            });

        });
        return Image;
    }

    public Image<Rgba32> DrawPath(IList<NodeXY> path,Color color)
    {
        Dictionary<(int, int), bool> drawn = new();
        Image.Mutate(x =>
        {
            path.Aggregate((n1, n2) =>
            {
                if (drawn.TryGetValue((n1.Id, n2.Id), out var _)) return n2;
                DrawEdge(x,new NodeConnector(n1,n2){Color=color}, x.GetCurrentSize());
                drawn[(n1.Id, n2.Id)] = true;
                return n2;
            });
        });
        return Image;
    }
    public void DrawNodesId(IList<NodeXY> nodes){
        Image.Mutate(x =>
        {
            Parallel.ForEach(nodes, node =>{
                DrawNodeId(x,node,x.GetCurrentSize());
            });
        });
    }
    public void DrawNodeId(IImageProcessingContext x, NodeXY nodeXY, Size ImageSize)
    {
        var point = new PointF((float)nodeXY.X * ImageSize.Width, (float)nodeXY.Y * ImageSize.Height);
        x.DrawText(nodeXY.Id.ToString(), Font, Color.Violet, point);
    }
    public void DrawNode(IImageProcessingContext x, NodeXY nodeXY, Size ImageSize, float nodeSize)
    {
        var brush = new SolidBrush(nodeXY.Color);
        var point = new PointF((float)nodeXY.X * ImageSize.Width, (float)nodeXY.Y * ImageSize.Height);
        var ellipse = new EllipsePolygon(point, nodeSize * Image.Height);
        x.FillPolygon(new DrawingOptions() { }, brush, ellipse.Points.ToArray());
    }
    public void DrawEdge(IImageProcessingContext x, NodeConnector edge, Size ImageSize)
    {
        var brush = new SolidBrush(edge.Color);
        var point1 = new PointF((float)edge.Parent.X * ImageSize.Width, (float)edge.Parent.Y * ImageSize.Height);
        var point2 = new PointF((float)edge.Node.X * ImageSize.Width, (float)edge.Node.Y * ImageSize.Height);
        x.DrawLines(new DrawingOptions() { }, brush, Thickness * ImageSize.Height, point1, point2);
    }
}