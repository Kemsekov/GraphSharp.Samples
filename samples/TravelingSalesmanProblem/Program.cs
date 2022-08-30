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

var mst = graph.Do.FindSpanningTree();
var low = mst.Sum(x=>x.Weight);

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

//we can find how good resulting hamiltonian cycle is by dividing 
//length of cycle by length of minimal spanning tree
System.Console.WriteLine("Rate " + edges.Sum(x => x.Weight) / low);


Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    // drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawPath(path,Color.Orange,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawEdgesParallel(edges, argz.thickness);
});