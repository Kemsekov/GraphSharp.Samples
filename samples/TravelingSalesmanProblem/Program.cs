using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using GraphSharp;
using GraphSharp.Extensions;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using MathNet.Numerics.LinearAlgebra.Single;
using SampleBase;
using Unchase.Satsuma.TSP;

double NodeDistance(Node n1, Node n2) => (n1.MapProperties().Position-n2.MapProperties().Position).L2Norm();
Vector Pos(Node n)=>n.MapProperties().Position;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation(x=>x.MapProperties().Position);

var mst = graph.Do.FindSpanningForestKruskal();
var low = mst.Forest.Sum(x=>x.Weight);

IEnumerable<Node> path = new List<Node>();
double cost = 0;
//google or tools variant. It is uncomparable slow =(
// Helpers.MeasureTime(() =>{
//     var scale = 10000L;
//     long dist(Node n1,Node n2)=>(long)(scale*NodeDistance(n1,n2));
//     System.Console.WriteLine("Solving tsp by google or tools");
//     graph.Do.DelaunayTriangulation(x=>x.MapProperties().Position);
//     graph.Do.TransitiveClosureOnRadius(2);
//     var distances = new ConcurrentDictionary<(int source,int target),long>();
//     foreach(var e in graph.Edges){
//         var s1 = Math.Min(e.SourceId,e.TargetId);
//         var s2 = Math.Max(e.SourceId,e.TargetId);
//         distances[(s1,s2)]=(long)(scale*NodeDistance(graph.Nodes[e.SourceId],graph.Nodes[e.TargetId]));
//     }
//     // var m = graph.Do.DistanceMatrix(dist);
//     var tsp = graph.Do.TspGoogleOrTools((n1,n2) => distances.GetOrDefault((Math.Min(n1,n2),Math.Max(n1,n2)),1000000000L));
//     System.Console.WriteLine(tsp.TourCost*1.0/scale/low);
//     path = tsp.Tour;
// });


Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Solving traveling salesman problem...");
    // var path1 = graph.Do.TspCheapestLink((n1,n2)=>(n1.Position-n2.Position).Length());
    var path1 = graph.Do.TspCheapestLinkOnEdgeCost(e=>e.Weight,g=>g.Do.DelaunayTriangulation(Pos,1e-8));
    // var path1 = graph.Do.TspCheapestLinkOnPositions(x=>x.Position);
    cost = path1.TourCost;
    System.Console.WriteLine("Rate " + cost / low);
    path = path1.Tour;
});

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Improving solution by opt2");
    var path1 = graph.Do.TspSmallRandomOpt2(path,cost,NodeDistance,400);

    // var path1 = graph.Do.TspOpt2(path,cost,NodeDistance);
    cost = TspUtils.GetTourCost(path1.Tour,NodeDistance);
    System.Console.WriteLine("Rate " + cost / low);
    path = path1.Tour;
});


Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.MapProperties().Position,(n,p)=>n.MapProperties().Position = p);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    // drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawPath(path,argz.thickness,Color.Orange);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
},x=>x.MapProperties().Position);