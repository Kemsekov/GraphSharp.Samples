using System.Diagnostics;
using System.Drawing;
using GraphSharp.Edges;
using GraphSharp.Graphs;
using GraphSharp.Nodes;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using MathNet.Numerics.LinearAlgebra.Single;
using SampleBase;

//(E+Vlog(V))V

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
// graph.Do.DelaunayTriangulation();
// graph.Do.MakeUndirected();
var components = graph.Do.FindStronglyConnectedComponents();
System.Console.WriteLine(components.Sum(x=>x.nodes.Count()));
System.Console.WriteLine(components.Count());
foreach(var c in components){
    var color = Color.FromArgb(Random.Shared.Next(256),Random.Shared.Next(256),Random.Shared.Next(256));
    foreach(var n in c.nodes){
        n.Color = color;
    }
}

if (false)
Helpers.MeasureTime(() =>
{
    var weights = new float[graph.Nodes.MaxNodeId + 1];
    var nextWeights = new float[graph.Nodes.MaxNodeId + 1];
    foreach (var n in graph.Nodes)
    {
        weights[n.Id] = 1;
        nextWeights[n.Id] = 1;
    }
    var gaveWeights = new byte[graph.Nodes.MaxNodeId+1];
    for (int i = 0; i < 5; i++)
    {
        foreach (var e in graph.Edges)
        {
            var weightToMove = weights[e.Target.Id]*e.Weight;
            nextWeights[e.Source.Id] += weightToMove;
            if(gaveWeights[e.Target.Id]!=0){
                nextWeights[e.Target.Id] -= weightToMove;
                gaveWeights[e.Target.Id] = 1;
            }
        }
        Array.Fill(gaveWeights,(byte)0);
        (nextWeights, weights) = (weights, nextWeights);
    }
    var min = weights.Min();
    var max = weights.Max()-min;
    var weightsScale = weights.Select(x => (x-min) / max).ToArray();
    foreach (var n in graph.Nodes)
    {
        var newRed = (int)(255 * weightsScale[n.Id]);
        n.Color = Color.FromArgb(newRed, newRed, newRed);
    }
});

if (false)
    Helpers.MeasureTime(() =>
    {
        System.Console.WriteLine("Approximating radius and center of a graph...");
        (var radius, var center, var approximationPath) = graph.Do.ApproximateCenter(339);
        foreach (var n in approximationPath)
            n.Color = Color.Orange;
        foreach (var n in center)
        {
            var e = graph.Do.FindEccentricity(n.Id);
            System.Console.WriteLine($"Center: {n} with eccentricity {e.length}");
            n.Color = Color.Red;
        }
        System.Console.WriteLine($"Radius is {radius}");

    });
if (false)
    Helpers.MeasureTime(() =>
    {
        System.Console.WriteLine("Finding radius and center of a graph...");
        (var radius, var center) = graph.Do.FindCenter();
        foreach (var n in center)
        {
            var e = graph.Do.FindEccentricity(n.Id);
            System.Console.WriteLine($"Center: {n} with eccentricity {e.length}");
        }
        System.Console.WriteLine($"Radius is {radius}");

    });

Helpers.CreateImage(argz, graph.Configuration, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawDirectionsParallel(graph.Edges,argz.thickness,argz.directionLength,Color.Orange);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    // drawer.DrawNodeIds(graph.Nodes, Color.Wheat, argz.fontSize);
});