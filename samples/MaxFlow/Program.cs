using QuikGraph.Algorithms;
using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Adapters;
using MathNet.Numerics.LinearAlgebra.Single;
using System.Text.RegularExpressions;
using Google.OrTools.Graph;
using MathNet.Numerics.Random;

//Here is a good example how ToQuikGraph adapter works in a real example
ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
Vector Pos(int node) => graph.Nodes[node].MapProperties().Position;
void SetPos(int node, Vector p) => graph.Nodes[node].MapProperties().Position = p;

//create some graph with source and sink
graph.Do.MakeBidirected();
graph.Do.TopologicalSort(65).ApplyTopologicalSort(SetPos, x => Pos(x)[1]);
graph.Do.MakeSources(65);
graph.Do.MakeDirected();

//set capacities
foreach (var e in graph.Edges)
{
    e.MapProperties().Capacity = Random.Shared.NextInt64(20);
}

MaxFlowResult<Edge>? maxFlow = null;

//find a max flow from source 65 to sink 39.
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Searching max flow...");

    //use some version of max flow computation
    var m1 = graph.Do.MaxFlowEdmondsKarp(65, 39);
    var m2 = graph.Do.MaxFlowGoogleOrTools(65, 39);

    maxFlow = m1;
    System.Console.WriteLine("Max flow is " + maxFlow.MaxFlow);
});
Helpers.ShiftNodesToFitInTheImage(graph.Nodes, x => x.MapProperties().Position, (n, p) => n.MapProperties().Position = p);
Helpers.CreateImage(argz, graph, drawer =>
{
    var flow = maxFlow?.Flow;
    drawer.Clear(Color.Black);
    foreach (var e in graph.Edges)
    {
        //the more red edge is, the more capacity it has
        var thickness = Math.Min((flow?[e] ?? 0)/5, 1);
        drawer.DrawEdge(e, argz.thickness, Color.FromArgb((int)(thickness * 255), 0, 0));
    }
    drawer.DrawNodeId(graph.Nodes[65], Color.Azure, argz.fontSize);
    drawer.DrawNodeId(graph.Nodes[39], Color.Azure, argz.fontSize);
}, x => Pos(x.Id));