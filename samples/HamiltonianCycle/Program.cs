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
argz.filename = "output.jpeg";
var g = Helpers.CreateGraph(argz);

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Creating ham cycle / connecting");
    g.Do.ConnectAsHamiltonianCycle(n => n.MapProperties().Position);
    g.Do.ConnectToClosest(5, L2);
});

System.Console.WriteLine(g.Edges.Count);
IEdgeSource<Edge> edgesInPath = new DefaultEdgeSource<Edge>();
IPath<Node>? path = null;
Helpers.MeasureTime(() =>
{
    var weights = (Edge e) => L2(g.Nodes[e.SourceId], g.Nodes[e.TargetId]); ;
    System.Console.WriteLine("Searching ham cycle");
    (path,edgesInPath) = g.Do.HamCycleDirected(weights, 100);
});

if(path is null) return;

System.Console.WriteLine(path.PathType);
g.ValidateCycle(path);


Helpers.CreateImage(argz, g, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(g.Edges, argz.thickness, Color.DarkViolet);
    drawer.DrawEdgesParallel(edgesInPath, argz.thickness, Color.Blue);
    drawer.DrawNodeIds(g.Nodes, Color.Azure, argz.fontSize);
}, x => (Vector)(g.Nodes[x.Id].MapProperties().Position * 0.9f + 0.05f));