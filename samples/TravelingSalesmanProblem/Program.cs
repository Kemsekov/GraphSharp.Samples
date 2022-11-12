using System.Diagnostics;
using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using MathNet.Numerics.LinearAlgebra.Single;
using SampleBase;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation();

var mst = graph.Do.FindSpanningForestKruskal();
var low = mst.Sum(x=>x.Weight);

IEnumerable<Node> path = new List<Node>();
double cost = 0;

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Solving traveling salesman problem...");
    (path, cost) = graph.Do.TspCheapestLink((n1,n2)=>(n1.Position-n2.Position).Length());
    System.Console.WriteLine("Rate " + cost / low);
});
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Improving solution by opt2");
    (path, cost) = graph.Do.TspOpt2(path,cost);
    System.Console.WriteLine("Rate " + cost / low);
});


Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    // drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawPath(path,Color.Orange,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
});