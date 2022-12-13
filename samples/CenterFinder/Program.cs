using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using GraphSharp;
using GraphSharp.Common;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using MathNet.Numerics.LinearAlgebra.Single;
using SampleBase;

//(E+Vlog(V))V fdghgdh 

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
// graph.Do.DelaunayTriangulation();
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding center by approximation (orange)");
    var c1 = graph.Do.TryFindCenterByApproximation(x=>1,false);
    System.Console.WriteLine($"Found {c1.center.Count()} center nodes with radius {c1.radius}");
    foreach(var n in c1.center){
        System.Console.WriteLine(n);
        n.Color = Color.Orange;
    }
});
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding center by dijkstras (blue)");
    var c2 = graph.Do.FindCenterByDijkstras(x=>1,false);
    System.Console.WriteLine($"Found {c2.center.Count()} center nodes with radius {c2.radius}");
    foreach(var n in c2.center){
        System.Console.WriteLine(n);
        n.Color = Color.Blue;
    }
});

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    // drawer.DrawDirectionsParallel(graph.Edges, argz.thickness, argz.directionLength, Color.Orange);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    // drawer.DrawNodeIds(graph.Nodes, Color.Wheat, argz.fontSize);

},x=>x.Position);