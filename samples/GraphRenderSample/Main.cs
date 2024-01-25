using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using GraphSharp;
using GraphSharp.GraphDrawer;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;

//this is example how to render graph in real-time.

namespace Test;
public class Render
{
    public Canvas Canvas;
    public Graph Graph;
    public ArgumentsHandler Argz { get; }
    public GraphDrawer<Node, Edge> Drawer { get; private set; }
    public CanvasShapeDrawer CanvasDrawer { get; }
    public PlanarGraphRender<Node, Edge> PlanarRender { get; }
    void Init()
    {

    }
    bool Done = false;
    void DoStuff()
    {
        if (Done)
        {
            return;
        }
        if (PlanarRender.ComputeStep())
        {
            UpdatePositions();
        }
        else
            Done = true;
    }

    private void UpdatePositions()
    {
        foreach (var d in PlanarRender.Positions)
            Graph.Nodes[d.Key].MapProperties().Position = (Vector)d.Value;
    }

    public Render(Canvas canvas)
    {
        Canvas = canvas;
        ArgumentsHandler Argz = new("settings.json");
        this.Argz = Argz;
        this.Graph = Helpers.CreateGraph(Argz);
        Graph.Do.DelaunayTriangulation(x => x.MapProperties().Position);
        // Graph.Do.ConnectToClosestParallel(10,(n1,n2)=>(n1.Position-n2.Position).Length());

        this.PlanarRender = new PlanarGraphRender<Node, Edge>(Graph, 5);

        var drawer = new CanvasShapeDrawer(Canvas);
        var graphDrawer = new GraphDrawer<Node, Edge>(Graph, drawer, 1000, x => x.MapProperties().Position);
        this.Drawer = graphDrawer;
        this.CanvasDrawer = drawer;
    }
    Vector Vec(float x, float y)=> new DenseVector(new[]{x,y});

    public async void RenderStuff()
    {
        while (true)
        {
            Drawer.Clear(System.Drawing.Color.Empty);

            Drawer.DrawEdges(Graph.Edges, Argz.thickness);
            // Drawer.DrawDirections(Graph.Edges, Argz.thickness, Argz.directionLength, System.Drawing.Color.Orange);
            Drawer.DrawNodes(Graph.Nodes, Argz.nodeSize);

            // Drawer.DrawNodes(PlanarRender.FixedPoints.Select(x => Graph.Nodes[x]), Argz.nodeSize);
            // Drawer.DrawNodeIds(Graph.Nodes, System.Drawing.Color.Azure, Argz.fontSize);
            CanvasDrawer.DrawText($"{PlanarRender.EdgesLengthSum.ToString("0.00")} sum of edges length", Vec(700, 50), System.Drawing.Color.Azure, 20);
            CanvasDrawer.Dispatch();
            await Task.Delay(Argz.renderIntervalMilliseconds);
        }

    }

    double shift = 0.1f;
    public void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Space:
                break;
            case Key.Escape:
                PlanarRender.ResetFixedPoints(5);
                UpdatePositions();
                Done = false;
                Task.Run(ComputeStuff);
                break;
            case Key.A:
                this.Drawer.XShift += shift / this.Drawer.SizeMult;
                break;
            case Key.D:
                this.Drawer.XShift -= shift / this.Drawer.SizeMult;
                break;
            case Key.W:
                this.Drawer.YShift += shift / this.Drawer.SizeMult;
                break;
            case Key.S:
                this.Drawer.YShift -= shift / this.Drawer.SizeMult;
                break;
            case Key.Up:
                this.Drawer.XShift -= shift / this.Drawer.SizeMult / 2;
                this.Drawer.YShift -= shift / this.Drawer.SizeMult / 2;
                this.Drawer.SizeMult = this.Drawer.SizeMult * (1 + shift);

                break;
            case Key.Down:
                this.Drawer.SizeMult = this.Drawer.SizeMult / (1 + shift);
                this.Drawer.XShift += shift / this.Drawer.SizeMult / 2;
                this.Drawer.YShift += shift / this.Drawer.SizeMult / 2;
                break;
            case Key.R:
                this.Drawer.SizeMult = 1;
                this.Drawer.XShift = 0;
                this.Drawer.YShift = 0;
                break;
        }
    }

    public async void ComputeStuff()
    {
        while (!Done)
        {
            DoStuff();
            await Task.Delay(Argz.computeIntervalMilliseconds);
        }
    }
}