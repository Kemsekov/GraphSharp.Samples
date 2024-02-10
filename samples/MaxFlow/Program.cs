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

Graph<Node,Edge> graph = Helpers.CreateGraph(argz);
Vector Pos(int node) => graph.Nodes[node].MapProperties().Position;
void SetPos(int node, Vector p) => graph.Nodes[node].MapProperties().Position = p;


//create some graph with source and sink
graph.Do.DelaunayTriangulation(x=>x.MapProperties().Position);
var largestSCC = graph.Do.FindStronglyConnectedComponentsTarjan().Components.MaxBy(x=>x.nodes.Count());
graph = graph.Do.Induce(largestSCC.nodes.Select(n=>n.Id));
var source  = graph.Nodes.First().Id;

graph.Do.MakeBidirected();
graph.Do.MakeSources(source);
graph.Do.MakeDirected();
var sink = graph.Do.FindEccentricity(source,e=>1).farthestNode.Id;
var render = graph.Do.Arrange(100);
foreach(var n in graph.Nodes){
    n.MapProperties().Position= (Vector)render[n.Id];
}
graph.Do.TopologicalSort(source).ApplyTopologicalSort(SetPos, x => Pos(x)[1]);

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
    var m1 = graph.Do.MaxFlowEdmondsKarp(source, sink);
    var m2 = graph.Do.MaxFlowGoogleOrTools(source, sink);

    maxFlow = m1;
    System.Console.WriteLine("Max flow is " + maxFlow.MaxFlow);
});
if(maxFlow is null) throw new Exception("failed to find max flow");

var minCut = graph.Do.MinCut(maxFlow);

Helpers.ShiftNodesToFitInTheImage(graph.Nodes, x => x.MapProperties().Position, (n, p) => n.MapProperties().Position = p);
Helpers.CreateImage(argz, graph, drawer =>
{
    var flow = maxFlow?.Flow;
    drawer.Clear(Color.Black);
    foreach (var e in graph.Edges)
    {
        //the more red edge is, the more capacity it has
        var thickness = Math.Min(0.1+(flow?[e] ?? 0)/5, 1);
        var color = Color.FromArgb((int)(thickness * 255), 0, 0);
        drawer.DrawEdge(e, argz.thickness, color);
        drawer.DrawDirection(e,argz.thickness,0.1,Color.FromArgb(color.G,color.R,color.B));
    }
    var leftCutColor = Color.Blue;
    var rightCutColor = Color.Green;
    var leftCut = minCut.LeftCutNodes.Select(i=>graph.Nodes[i]);
    var rightCut = minCut.RightCutNodes.Select(i=>graph.Nodes[i]);

    drawer.DrawNodes(leftCut,argz.nodeSize,leftCutColor);
    drawer.DrawNodes(rightCut,argz.nodeSize,rightCutColor);

    drawer.DrawNodeId(graph.Nodes[source], Color.Azure, argz.fontSize);
    drawer.DrawNodeId(graph.Nodes[sink], Color.Azure, argz.fontSize);
}, x => Pos(x.Id));