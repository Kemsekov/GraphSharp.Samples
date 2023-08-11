using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using GraphSharp;
using GraphSharp.GraphDrawer;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;

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
            Graph.Nodes[d.Key].Position = (Vector)d.Value;
    }

    public Render(Canvas canvas)
    {
        Canvas = canvas;
        ArgumentsHandler Argz = new("settings.json");
        this.Argz = Argz;
        this.Graph = Helpers.CreateGraph(Argz);
        Graph.Do.DelaunayTriangulation(x => x.Position);
        // Graph.Do.ConnectToClosestParallel(10,(n1,n2)=>(n1.Position-n2.Position).Length());

        this.PlanarRender = new PlanarGraphRender<Node, Edge>(Graph, 5);
        ResetColors();

        var drawer = new CanvasShapeDrawer(Canvas);
        var graphDrawer = new GraphDrawer<Node, Edge>(Graph, drawer, 1000, x => x.Position);
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
                ShowHideFixedPointsBorder();
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

    private void ShowHideFixedPointsBorder()
    {
        foreach (var n in Graph.Edges.InducedEdges(PlanarRender.FixedPoints))
        {
            if (n.Color == Color.Empty)
                n.Color = Color.Green;
            else
                n.Color = Color.Empty;
        }
        PlanarRender.FixedPoints.Aggregate((n1, n2) =>
        {
            if (Graph.Edges.BetweenOrDefault(n1, n2) is Edge e)
            {
                if (e.Color == Color.Empty)
                    e.Color = Color.Aqua;
                else
                    e.Color = Color.Empty;
            }
            return n2;
        });
        var n1 = PlanarRender.FixedPoints.First();
        var n2 = PlanarRender.FixedPoints.Last();
        if (Graph.Edges.BetweenOrDefault(n1, n2) is Edge e)
        {
            if (e.Color == Color.Empty)
                e.Color = Color.Aqua;
            else
                e.Color = Color.Empty;
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

    internal void OnPointerPressed(PointerPressedEventArgs e)
    {
        AddToCircle(e.GetPosition(Canvas));
    }

    void AddToCircle(Avalonia.Point pos){
        var vec = new DenseVector(new float[]{(float)(pos.X / Drawer.Size),(float)(pos.Y / Drawer.Size)});
        var closest = Graph.Nodes.Where(x => x.Color == Color.Red).MinBy(x => (x.Position - vec).L2Norm());
        if (closest is null) return;

        var oldFixedPoints = PlanarRender.FixedPoints;
        var newFixedPoints = new List<int>();
        int FillNewFixedPoints(int n1, int n2, ref Node? closest, List<int> newFixedPoints)
        {
            newFixedPoints.Add(n1);
            var common = Graph.Edges.Neighbors(n1).Intersect(Graph.Edges.Neighbors(n2)).ToList();
            if (closest is not null)
                if (common.Contains(closest.Id))
                {
                    newFixedPoints.Add(closest.Id);
                    closest = null;
                }
            return n2;
        }
        oldFixedPoints.Aggregate((n1, n2) =>
        {
            return FillNewFixedPoints(n1, n2, ref closest, newFixedPoints);
        });

        FillNewFixedPoints(oldFixedPoints.Last(),oldFixedPoints.First(),ref closest, newFixedPoints);

        PlanarRender.ResetFixedPoints(newFixedPoints.ToArray());
        ResetColors();
        Done = false;
        Task.Run(ComputeStuff);
    }

    private void ResetColors()
    {
        Graph.Edges.SetColorToAll(Color.DarkViolet);
        Graph.Nodes.SetColorToAll(Color.Empty);
        ShowHideFixedPointsBorder();
        
        PlanarRender.FixedPoints.Aggregate((n1, n2) =>
        {
            var intersections = Graph.Edges.Neighbors(n1).Intersect(Graph.Edges.Neighbors(n2)).ToList();
            if (intersections.Count != 0)
            {
                foreach (var n in intersections)
                    if (!PlanarRender.FixedPoints.Contains(n))
                        Graph.Nodes[n].Color = Color.Red;
            }
            return n2;
        });
    }

    internal void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        AddToCircle(e.GetPosition(Canvas));
    }
}