using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using GraphSharp;
using GraphSharp.Common;
using GraphSharp.GraphDrawer;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Test;
public class Render
{
    public Canvas Canvas;
    public Graph<Node,NeuralEdge> Graph;
    public ArgumentsHandler Argz { get; }
    public GraphDrawer<Node, NeuralEdge> Drawer { get; private set; }
    public CanvasShapeDrawer CanvasDrawer { get; }
    public NeuralVisitor<Node> Visitor { get; }
    public Propagator<Node, NeuralEdge> Propagator { get; }
    public int[] InputSet { get; }
    public int[] OutputSet { get; }
    public int[] TrainSet { get; }
    public GraphArrange<Node, NeuralEdge> GraphArrange { get; }
    public Dictionary<int, Vector> Positions { get; }
    public int Steps { get; private set; }

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
        Done = true;
        if (GraphArrange.ComputeStep() > 0.01)
        {
            UpdatePositions();
            allowRender = true;
            Steps+=1;
        }
    }
    Vector Vec(float x, float y)=> new DenseVector(new[]{x,y});

    private void UpdatePositions()
    {
        try{
            foreach (var n in Graph.Nodes)
            {
                var pos = GraphArrange[n.Id];
                n.Position = Vec(pos[0], pos[1]);
            }
        }
        catch(Exception e){
            System.Console.WriteLine(e.Message);
        }
    }

    public Render(Canvas canvas)
    {
        Canvas = canvas;
        ArgumentsHandler Argz = new("settings.json");
        this.Argz = Argz;
        Graph = new Graph<Node,NeuralEdge>(x=>new Node(x){Position=Vec(Random.Shared.NextSingle(),Random.Shared.NextSingle())},(s,t)=>new NeuralEdge(s.Id,t.Id,0));
        Graph.Do.CreateNodes(Argz.nodesCount);
        // Graph.Do.DelaunayTriangulation(x=>x.Position);
        Graph.Do.ConnectToClosestParallel(6,(n1,n2)=>(n1.Position-n2.Position).L2Norm());
        // Graph.Do.ConnectRandomly(4,10);
        foreach(var node in Graph.Nodes){
            var edges = Graph.Edges.OutEdges(node.Id);
            foreach(var edge in edges)
                edge.Weight = Random.Shared.NextDouble();
            var sum = edges.Sum(x=>x.Weight);
            foreach(var edge in edges)
                edge.Weight/=sum;
            sum = edges.Sum(x=>x.Weight);
            if(sum-1>0.0001)
                throw new Exception("Bad: "+sum+"!="+1);
        }
        Visitor = new NeuralVisitor<Node>(Graph);
        Propagator = new Propagator<Node,NeuralEdge>(Visitor,Graph);

        this.InputSet = Enumerable.Range(0,100).ToArray();
        this.OutputSet = Enumerable.Range(100,200).ToArray();
        this.TrainSet = Enumerable.Range(200,300).ToArray();

        Propagator.SetPosition(Enumerable.Range(0,100).ToArray());
        GraphArrange = new GraphArrange<Node, NeuralEdge>(Graph, Graph.Nodes.Count, 2){DistancePower=2};

        Positions = GraphArrange.Positions;
        allowRender = true;

        var drawer = new CanvasShapeDrawer(Canvas);
        var graphDrawer = new GraphDrawer<Node, NeuralEdge>(Graph, drawer, 1000, x => x.Position);
        this.Drawer = graphDrawer;
        this.CanvasDrawer = drawer;
    }
    bool allowRender = false;
    public async void RenderStuff()
    {
        while (true)
        {
            if (allowRender)
            {
                Drawer.Clear(System.Drawing.Color.Empty);
                Drawer.DrawEdges(Graph.Edges, Argz.thickness);
                // Drawer.DrawDirections(Graph.Edges, Argz.thickness, Argz.directionLength, System.Drawing.Color.Orange);
                // Drawer.DrawNodes(Graph.Nodes, Argz.nodeSize);

                // Drawer.DrawNodes(PlanarRender.FixedPoints.Select(x => Graph.Nodes[x]), Argz.nodeSize);
                // Drawer.DrawNodeIds(Graph.Nodes, System.Drawing.Color.Azure, Argz.fontSize);
                // CanvasDrawer.DrawText($"{PlanarRender.EdgesLengthSum.ToString("0.00")} sum of edges length", Vec(700, 50), System.Drawing.Color.Azure, 20);
                CanvasDrawer.DrawText($"{GraphArrange.EdgesLengthSum.ToString("0.00")} sum of edges length", Vec(700, 50), System.Drawing.Color.Azure, 20);
                CanvasDrawer.DrawText($"{GraphArrange.DistancePower.ToString("0.00")} distance power", Vec(700, 70), System.Drawing.Color.Azure, 20);
                CanvasDrawer.DrawText($"{Steps} steps", Vec(700, 90), System.Drawing.Color.Azure, 20);
                CanvasDrawer.Dispatch();
                allowRender = false;
            }
            await Task.Delay(Argz.renderIntervalMilliseconds);
        }

    }
    double shift = 0.1f;
    public void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.E:
                Done = false;
                Task.Run(DoStuff);
                break;
            case Key.NumPad0:
                GraphArrange.DistancePower=0.5f;
                break;
            case Key.NumPad1:
                GraphArrange.DistancePower=1;
                break;
            case Key.NumPad2:
                GraphArrange.DistancePower=2;
                break;
            case Key.Escape:
                var r = new Random(0);
                foreach(var v in GraphArrange.Positions){
                    v.Value[0] = r.NextSingle();
                    v.Value[1] = r.NextSingle();
                }
                Steps = 0;
                Done = false;
                Task.Run(DoStuff);
                break;

            case Key.P:
                Propagator.NodeStates.AddState(UsedNodeStates.Visited,InputSet);
                break;
            case Key.Space:
                Propagator.Propagate();
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
        allowRender = true;
    }
    public async void ComputeStuff()
    {
        while (!Done)
        {
            // DoStuff();
            await Task.Delay(Argz.computeIntervalMilliseconds);
        }
    }

    internal void OnPointerPressed(PointerPressedEventArgs e)
    {
    }


    internal void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
    }
}