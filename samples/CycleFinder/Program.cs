using System.Drawing;
using GraphSharp.GraphStructures;

//This sample shows how easy it is to find cycles basis!

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);

graph.CheckForIntegrity();
var cycles = Enumerable.Empty<IList<NodeXY>>();
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine($"Finding cycles basis...");
    cycles = graph.Do.FindCyclesBasis();
    System.Console.WriteLine($"Found {cycles.Count()} cycles!");
});
//validate that found cycles is correct and print it
foreach (var c in cycles){
    graph.ValidateCycle(c);
    Helpers.PrintPath(c);
}
//color cycles so we can see them on the image
foreach (var cycle in cycles)
{
    var color = Color.FromArgb(Random.Shared.Next(256), Random.Shared.Next(256), Random.Shared.Next(256));
    cycle.Aggregate((n1, n2) =>
    {
        var edge = graph.Edges[n1.Id, n2.Id];
        if (edge.Color == NodeConnector.DefaultColor)
        {
            edge.Color = color;
            if (graph.Edges.TryGetEdge(n2.Id, n1.Id, out var undirected) && undirected is not null)
                undirected.Color = color;
        }
        return n2;
    });
}

Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz, graph.Configuration, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes, Color.Wheat, argz.fontSize);
});
