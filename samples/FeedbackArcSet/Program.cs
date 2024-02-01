using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);

graph.Do.DelaunayTriangulation(x => x.MapProperties().Position);

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Is DAG: " + graph.IsDirectedAcyclic());

    System.Console.WriteLine("Finding feedback arc set");
    var feedbackArcSet = graph.Do.FeedbackArcSet(x => x.Weight);
    System.Console.WriteLine("Removing edges from feedback arc set...");
    graph.Do.RemoveEdges(e => feedbackArcSet.BetweenOrDefault(e.SourceId, e.TargetId) is not null);
    System.Console.WriteLine("Removed "+feedbackArcSet.Count);

    System.Console.WriteLine("Is DAG: " + graph.IsDirectedAcyclic());
});
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawDirectionsParallel(graph.Edges, argz.thickness, 0.1, Color.Gray);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
}, x => (Vector)(x.MapProperties().Position * 0.9f + 0.05f));
