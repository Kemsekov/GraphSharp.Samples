
using System.Drawing;
using GraphSharp.Graphs;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation();

var colors =
    new[] { Color.Azure, Color.Yellow, Color.Red, Color.Coral, Color.Blue, Color.Violet, Color.Aqua };
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Greedy coloring graph...");
    var usedColors = graph.Do.GreedyColorNodes(colors);
    System.Console.WriteLine($"Total colors used : {usedColors.Where(x => x.Value != 0).Count()}");
    foreach (var colorInfo in usedColors.OrderByDescending(x => x.Value))
    {
        System.Console.WriteLine(colorInfo);
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x => x.Value));
});
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("DSatur coloring graph...");
    var usedColors = graph.Do.DSaturColorNodes(colors);
    System.Console.WriteLine($"Total colors used : {usedColors.Where(x => x.Value != 0).Count()}");
    foreach (var colorInfo in usedColors.OrderByDescending(x => x.Value))
    {
        System.Console.WriteLine(colorInfo);
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x => x.Value));
});

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Recursive largest first (RLF) coloring graph...");
    var usedColors = graph.Do.RLFColorNodes(colors);
    System.Console.WriteLine($"Total colors used : {usedColors.Where(x => x.Value != 0).Count()}");
    foreach (var colorInfo in usedColors.OrderByDescending(x => x.Value))
    {
        System.Console.WriteLine(colorInfo);
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x => x.Value));
});

graph.EnsureRightColoring();
// if(false)
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(graph.Edges, argz.thickness);
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
});
