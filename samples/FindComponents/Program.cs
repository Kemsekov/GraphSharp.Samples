using System.Drawing;
using GraphSharp.Graphs;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding components...");
    (var components,var setFinder) = graph.Do.FindComponents();
    System.Console.WriteLine($"Found {components.Count()} components");
    foreach (var c in components)
    {
        var color = Color.FromArgb(Random.Shared.Next(256), Random.Shared.Next(256), Random.Shared.Next(256));
        foreach (var n in c)
        {
            n.Color = color;
        }
    }
});
Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz,graph.Configuration,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
});
