using GraphSharp.Nodes;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Fonts;
using System.Reflection;

/// <summary>
/// Class for drawing graphs.
/// </summary>
public class GraphDrawer
{
    public float Thickness;
    public float NodeSize;
    public float DirectionLength = 0.4f;
    public Image<Rgba32> Image;
    public Font Font;
    float _fontSize;
    public GraphDrawer(Image<Rgba32> image, float fontSize)
    {
        this._fontSize = fontSize;
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
    public void DrawDirections(IList<NodeXY> nodes){
        Image.Mutate(x=>{
            Parallel.ForEach(nodes,node=>{
                foreach(var e in node.Edges)
                    DrawDirection(x,e,x.GetCurrentSize());
            });
        });
    }
    public void DrawNodeId(IImageProcessingContext x, NodeXY nodeXY, Size ImageSize)
    {
        var point = new PointF((float)(nodeXY.X-_fontSize/2) * ImageSize.Width, (float)(nodeXY.Y-_fontSize/2) * ImageSize.Height);
        x.DrawText(nodeXY.Id.ToString(), Font, nodeXY.Color, point);
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
        var point2 = new PointF((float)edge.Child.X * ImageSize.Width, (float)edge.Child.Y * ImageSize.Height);
        x.DrawLines(new DrawingOptions() { }, brush, Thickness * ImageSize.Height, point1, point2);
    }
    float Distance(PointF f){
        return MathF.Sqrt(f.X*f.X+f.Y*f.Y);
    }
    public void DrawDirection(IImageProcessingContext x, NodeConnector edge, Size ImageSize){
        var brush = new SolidBrush(edge.DirectionColor);
        var point1 = new PointF((float)edge.Parent.X * ImageSize.Width, (float)edge.Parent.Y * ImageSize.Height);
        var point2 = new PointF((float)edge.Child.X * ImageSize.Width, (float)edge.Child.Y * ImageSize.Height);
        var dirVector = point1-point2;
        var distance = Distance(dirVector);
        dirVector/=distance;
        dirVector.X*=ImageSize.Width;
        dirVector.Y*=ImageSize.Height;
        var point3=point2+dirVector*(DirectionLength);
        if(Distance(point2-point3)<distance)
            x.DrawLines(new DrawingOptions() { }, brush, Thickness * ImageSize.Height, point2, point3);
        else
            x.DrawLines(new DrawingOptions() { }, brush, Thickness * ImageSize.Height, point1, point2+(point1-point2)/2);
    }
}