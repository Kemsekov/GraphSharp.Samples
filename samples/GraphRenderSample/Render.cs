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

    int[] fixedPoints;
    IDictionary<int,Vector2> Acceleration;
    void Init()
    {

    }
    void DoStuff()
    {
        var nodes = Graph.Nodes;
        var edges = Graph.Edges;
        Helpers.NormalizeEdgesWeights(Graph.Edges);

        Parallel.ForEach(nodes, n =>
        {
            if(fixedPoints.Contains(n.Id)) return;
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
        });

        foreach(var n in Graph.Nodes){
            var shift = Argz.computeIntervalMilliseconds*Acceleration[n.Id]/1000;
            // var shift = Acceleration[n.Id];
            n.Position += shift;
            Acceleration[n.Id]*=0.99f;
        }
    }

    public Render(Canvas canvas)
    {
        Canvas = canvas;
        Acceleration = new ConcurrentDictionary<int,Vector2>();
        fixedPoints = new int[0];
        ArgumentsHandler Argz = new("settings.json");
        this.Argz = Argz;
        this.Graph = Helpers.CreateGraph(Argz);
        Graph.Do.DelaunayTriangulation();
        // Graph.Do.ConnectNodes(6);
        FindFixedPoints(3);

        foreach(var n in Graph.Nodes){
            Acceleration[n.Id] = new(0,0);
        }


        Helpers.NormalizeNodePositions(Graph.Nodes);
    }
    public async void RenderStuff()
    {
        var drawer = new CanvasShapeDrawer(Canvas);
        var graphDrawer = new GraphDrawer<Node, Edge>(Graph, drawer,1000);
        this.Drawer = graphDrawer;
        while (true)
        {
            graphDrawer.Clear(System.Drawing.Color.Empty);
            graphDrawer.DrawEdges(Graph.Edges, Argz.thickness);
            // graphDrawer.DrawDirections(Graph.Edges,Argz.thickness,Argz.directionLength,System.Drawing.Color.Orange);
            graphDrawer.DrawNodes(Graph.Nodes,Argz.nodeSize);
            graphDrawer.DrawNodes(fixedPoints.Select(x=>Graph.Nodes[x]),Argz.nodeSize);
            graphDrawer.DrawNodeIds(Graph.Nodes,System.Drawing.Color.Azure,Argz.fontSize);
            drawer.DrawText($"{GetEdgesLengthSum().ToString("0.00")} sum of edges length",new(700,50),System.Drawing.Color.Azure,20);
            drawer.Dispatch();
            await Task.Delay(Argz.renderIntervalMilliseconds);
        }

    }

    float shift = 0.1f;
    public void OnKeyDown(KeyEventArgs e){
        
        switch(e.Key){
            case Key.A:
                this.Drawer.XShift+=shift;
            break;
            case Key.D:
                this.Drawer.XShift-=shift;
            break;
            case Key.W:
                this.Drawer.YShift+=shift;
            break;
            case Key.S:
                this.Drawer.YShift-=shift;
            break;
            case Key.Up:
                this.Drawer.SizeMult+=shift;
            break;
            case Key.Down:
                this.Drawer.SizeMult-=shift;
            break;
            case Key.R:
                this.Drawer.SizeMult=1;
                this.Drawer.XShift=0;
                this.Drawer.YShift=0;
            break;
        }
    }
    public async void ComputeStuff()
    {
        while (true)
        {
            DoStuff();
            await Task.Delay(Argz.computeIntervalMilliseconds);
        }
    }
    public IList<Vector2> GenerateCoordinatesFor(int n){
        var coords = new List<Vector2>();
        var step = 2.0f*MathF.PI/(n-1);
        var value = step;
        for(int i = 0;i<n;i++){
            coords.Add(new Vector2(MathF.Cos(value),MathF.Sin(value)));
            value+=step;
        }
        return coords;
    }
    public void FindFixedPoints(int count){
        count += 1;
        foreach(var e in Graph.Edges){
            var p = Graph.Do.FindAnyPath(e.TargetId,e.SourceId,x=>!x.Equals(e));
            if(p.Count==count){
                fixedPoints = p.Select(x=>x.Id).ToArray();
                break;
            }
        }
        foreach(var n in fixedPoints.Zip(GenerateCoordinatesFor(fixedPoints.Length))){
            Graph.Nodes[n.First].Color = System.Drawing.Color.Orange;
            Graph.Nodes[n.First].Position = n.Second*100;
        }
    }
    float GetEdgesLengthSum(){
        return Graph.Edges.Sum(x=>(Graph.Nodes[x.SourceId].Position-Graph.Nodes[x.TargetId].Position).Length());
    }
}