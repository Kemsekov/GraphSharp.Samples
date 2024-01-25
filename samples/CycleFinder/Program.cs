using System.Drawing;
using GraphSharp;
using GraphSharp.Common;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;

//This sample shows how easy it is to find cycles basis!

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation(x=>x.MapProperties().Position);
graph.Do.MakeBidirected();

var cycles = Enumerable.Empty<IPath<Node>>();
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine($"Finding cycles basis...");
    cycles = graph.Do.FindCyclesBasis();
    System.Console.WriteLine($"Found {cycles.Count()} cycles!");
});
//validate that found cycles is correct and print it
foreach (var c in cycles)
{
    graph.ValidateCycle(c);
}

var orderedCycles = cycles.OrderBy(x => x.Count).ToList();

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes, Color.Wheat, argz.fontSize);
    //draw first 10 shortest found cycles
    foreach (var cycle in orderedCycles.Take(10))
    {
        var color = Color.FromArgb(255, Random.Shared.Next(256), Random.Shared.Next(256));
        drawer.DrawPath(cycle,argz.thickness,color);
    }
},x=> (Vector)(x.MapProperties().Position*0.9f+0.05f));
