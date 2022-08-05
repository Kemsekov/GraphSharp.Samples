using System.Diagnostics;
using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using MathNet.Numerics.LinearAlgebra.Single;
using SampleBase;

//(E+Vlog(V))V

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);


graph.Edges.SetColorToAll(Color.DarkViolet);
graph.Nodes.SetColorToAll(Color.Brown);
IEdgeSource<Edge> edges = new DefaultEdgeSource<Edge>();
IList<Node> path = new List<Node>();
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Solving traveling salesman problem...");
    (edges, path) = graph.Do.TravelingSalesmanProblem();
});
foreach (var e in edges)
{
    e.Color = Color.Orange;
}
System.Console.WriteLine("Average edge weight is " + edges.Sum(x => x.Weight) / edges.Count);


Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawEdgesParallel(edges, argz.thickness);
    // drawer.DrawPath(path,Color.Orange,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    // drawer.DrawNodeIds(graph.Nodes, Color.Wheat, argz.fontSize);
});