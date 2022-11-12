using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using GraphSharp;
using GraphSharp.GraphDrawer;
using GraphSharp.Graphs;

namespace Test;
public class Render
{
    public Canvas Canvas;
    public Graph Graph;
    public ArgumentsHandler Argz { get; }
    public GraphDrawer<Node, Edge> Drawer { get; private set; }
    public CanvasShapeDrawer CanvasDrawer { get; }
    public double EdgesLengthSum { get; private set; }
    public double Change = 1;
    int[] fixedPoints;
    IDictionary<int, Vector2> Acceleration;
    void Init()
    {

    }
    bool Done = false;
    void DoStuff()
    {
        if(Done) return;
        var edgesLengthSum = EdgesLengthSum;
        EdgesLengthSum = GetEdgesLengthSum();
        Change = Math.Abs(edgesLengthSum-EdgesLengthSum);
        if(Change<double.Epsilon){
            Done = true;
            System.Console.WriteLine("Done!");
            return;
        }
        var nodes = Graph.Nodes;
        var edges = Graph.Edges;
        var averageNodeDistance = edges.Average(x=>(nodes[x.SourceId].Position-nodes[x.TargetId].Position).Length());
        Parallel.ForEach(nodes, n =>
        {
            if (fixedPoints.Contains(n.Id)) return;
            Vector2 direction = new(0, 0);
            var outEdges = edges.OutEdges(n.Id);
            var inEdges = edges.InEdges(n.Id);
            foreach (var e in outEdges)
            {
                var dir = nodes[e.TargetId].Position - nodes[e.SourceId].Position;
                direction += dir;
            }
            foreach (var e in inEdges)
            {
                var dir = nodes[e.SourceId].Position - nodes[e.TargetId].Position;
                direction += dir;
            }
            Acceleration[n.Id] += direction;
            n.Position += direction / edges.Degree(n.Id);
        });

        Parallel.ForEach(nodes, n =>
        {
            var accel = Acceleration[n.Id];
            var shift = accel * Argz.computeIntervalMilliseconds / 1000;
            n.Position += shift;
            Acceleration[n.Id] *=(1-averageNodeDistance);
        });
    }

    public Render(Canvas canvas)
    {
        Canvas = canvas;
        Acceleration = new ConcurrentDictionary<int, Vector2>();
        fixedPoints = new int[0];
        ArgumentsHandler Argz = new("settings.json");
        this.Argz = Argz;
        this.Graph = Helpers.CreateGraph(Argz);
        Graph.Do.DelaunayTriangulation(x=>x.Position);
        // Graph.Do.ConnectNodes(6);
        foreach (var n in Graph.Nodes)
        {
            Acceleration[n.Id] = new(0, 0);
        }
        foreach(var n in Graph.Nodes) 
            n.Position = new(Random.Shared.NextSingle(), Random.Shared.NextSingle());
        FindFixedPoints(5);
        Helpers.NormalizeNodePositions(Graph.Nodes);
        var drawer = new CanvasShapeDrawer(Canvas);
        var graphDrawer = new GraphDrawer<Node, Edge>(Graph, drawer, 1000,x=>x.Position);
        this.Drawer = graphDrawer;
        this.CanvasDrawer = drawer;
    }
    public async void RenderStuff()
    {
        while (true)
        {
            Drawer.Clear(System.Drawing.Color.Empty);
            Drawer.DrawEdges(Graph.Edges, Argz.thickness);
            // Drawer.DrawDirections(Graph.Edges, Argz.thickness, Argz.directionLength, System.Drawing.Color.Orange);
            // Drawer.DrawNodes(Graph.Nodes, Argz.nodeSize);
            Drawer.DrawNodes(fixedPoints.Select(x => Graph.Nodes[x]), Argz.nodeSize);
            // Drawer.DrawNodeIds(Graph.Nodes, System.Drawing.Color.Azure, Argz.fontSize);
            CanvasDrawer.DrawText($"{EdgesLengthSum.ToString("0.00")} sum of edges length", new(700, 50), System.Drawing.Color.Azure, 20);
            CanvasDrawer.Dispatch();
            await Task.Delay(Argz.renderIntervalMilliseconds);
        }

    }

    double shift = 0.1f;
    public void OnKeyDown(KeyEventArgs e)
    {

        switch (e.Key)
        {
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
                this.Drawer.XShift -= shift / this.Drawer.SizeMult/2;
                this.Drawer.YShift -= shift / this.Drawer.SizeMult/2;
                this.Drawer.SizeMult = this.Drawer.SizeMult*(1+shift);

                break;
            case Key.Down:
                this.Drawer.SizeMult = this.Drawer.SizeMult/(1+shift);
                this.Drawer.XShift += shift / this.Drawer.SizeMult/2;
                this.Drawer.YShift += shift / this.Drawer.SizeMult/2;
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
    public IList<Vector2> GenerateCoordinatesFor(int n)
    {
        var coords = new List<Vector2>();
        var step = 2.0f * MathF.PI / (n - 1);
        var value = step;
        for (int i = 0; i < n; i++)
        {
            coords.Add(new Vector2(MathF.Cos(value), MathF.Sin(value)));
            value += step;
        }
        return coords;
    }
    public void FindFixedPoints(int count)
    {
        count += 1;
        foreach (var e in Graph.Edges)
        {
            var p = Graph.Do.FindAnyPath(e.TargetId, e.SourceId, x => !x.Equals(e)).Path;
            if (p.Count() == count)
            {
                fixedPoints = p.Select(x => x.Id).ToArray();
                break;
            }
        }
        foreach (var n in fixedPoints.Zip(GenerateCoordinatesFor(fixedPoints.Length)))
        {
            Graph.Nodes[n.First].Color = System.Drawing.Color.Orange;
            Graph.Nodes[n.First].Position = n.Second;
        }
    }
    double GetEdgesLengthSum()
    {
        return Graph.Edges.Sum(x => (Graph.Nodes[x.SourceId].Position - Graph.Nodes[x.TargetId].Position).Length());
    }
}