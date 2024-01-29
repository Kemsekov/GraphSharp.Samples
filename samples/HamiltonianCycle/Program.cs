using System.Drawing;
using System.Runtime.Serialization;
using Google.OrTools.LinearSolver;
using Google.Protobuf.Reflection;
using GraphSharp;
using GraphSharp.Common;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;

double L2(Node n1, Node n2) => (n1.MapProperties().Position - n2.MapProperties().Position).L2Norm();

ArgumentsHandler argz = new("settings.json");
argz.filename = "output.jpg";
var g = Helpers.CreateGraph(argz);

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Creating ham cycle / connecting");
    // g.Do.ConnectAsHamiltonianCycle(x=>x.MapProperties().Position);
    g.Do.ConnectToClosest(5, L2);
    g.Do.DelaunayTriangulation(x=>x.MapProperties().Position);
    g.Do.MakeBidirected();
});

System.Console.WriteLine(g.Edges.Count);
IEdgeSource<Edge> edgesInPath = new DefaultEdgeSource<Edge>();
IEnumerable<IPath<Node>>? cycles = null;
Helpers.MeasureTime(() =>
{
    var weights = (Edge e) => L2(g.Nodes[e.SourceId], g.Nodes[e.TargetId]); ;
    System.Console.WriteLine("Searching ham cycle");
    // (cycles,edgesInPath) = g.Do.HamCycleDirected(weights, 100);
    (cycles,edgesInPath) = g.Do.HamCycleUndirected(weights, 100);
});
System.Console.WriteLine("Edges "+edgesInPath.Count);


if(cycles is not null)
foreach(var c in cycles){
    g.ValidateCycle(c);
    System.Console.WriteLine("cycle "+c.Count);
}

Helpers.CreateImage(argz, g, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(g.Edges, argz.thickness, Color.DarkViolet);
    drawer.DrawEdgesParallel(edgesInPath, argz.thickness, Color.Yellow);
    // drawer.DrawNodeIds(g.Nodes, Color.Azure, argz.fontSize);
}, x => (Vector)(g.Nodes[x.Id].MapProperties().Position * 0.9f + 0.05f));