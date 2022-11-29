
using System.Drawing;
using GraphSharp.Adapters;
using GraphSharp.Graphs;
using QuikGraph.Algorithms.VertexColoring;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
if (graph.Edges.Count == 0)
    graph.Do.DelaunayTriangulation(x => x.Position);

var colors =
    new[] { Color.Azure, Color.Yellow, Color.Red, Color.Coral, Color.Blue, Color.Violet, Color.Aqua,Color.Green,Color.Orange,Color.Brown,Color.Aquamarine };
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Greedy coloring graph...");
    var coloring = graph.Do.GreedyColorNodes();
    var usedColors = coloring.CountUsedColors();
    System.Console.WriteLine($"Total colors used : {usedColors.Where(x => x.Value != 0).Count()}");
    foreach (var colorInfo in usedColors.OrderByDescending(x => x.Value))
    {
        System.Console.WriteLine(colorInfo);
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x => x.Value));
    coloring.ApplyColors(graph.Nodes,colors);
    try
    {
        graph.EnsureRightColoring();
    }
    catch (Exception ex)
    {
        System.Console.WriteLine(ex.Message);
    }
    graph.Nodes.SetColorToAll(Color.Empty);
});
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("DSatur coloring graph...");
    var coloring = graph.Do.DSaturColorNodes();
    var usedColors = coloring.CountUsedColors();
    System.Console.WriteLine($"Total colors used : {usedColors.Where(x => x.Value != 0).Count()}");
    foreach (var colorInfo in usedColors.OrderByDescending(x => x.Value))
    {
        System.Console.WriteLine(colorInfo);
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x => x.Value));
    coloring.ApplyColors(graph.Nodes,colors);
    try
    {
        graph.EnsureRightColoring();
    }
    catch (Exception ex)
    {
        System.Console.WriteLine(ex.Message);
    }
    graph.Nodes.SetColorToAll(Color.Empty);
});
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("QuikGraph coloring graph...");
    var coloring = graph.Do.QuikGraphColorNodes();
    var usedColors = coloring.CountUsedColors();
    System.Console.WriteLine($"Total colors used : {usedColors.Where(x => x.Value != 0).Count()}");
    foreach (var colorInfo in usedColors.OrderByDescending(x => x.Value))
    {
        System.Console.WriteLine(colorInfo);
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x => x.Value));
    coloring.ApplyColors(graph.Nodes,colors);
    try
    {
        graph.EnsureRightColoring();
    }
    catch (Exception ex)
    {
        System.Console.WriteLine(ex.Message);
    }
    graph.Nodes.SetColorToAll(Color.Empty);
});
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Recursive largest first (RLF) coloring graph...");
    var coloring = graph.Do.RLFColorNodes();
    var usedColors = coloring.CountUsedColors();
    System.Console.WriteLine($"Total colors used : {usedColors.Where(x => x.Value != 0).Count()}");
    foreach (var colorInfo in usedColors.OrderByDescending(x => x.Value))
    {
        System.Console.WriteLine(colorInfo);
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x => x.Value));
    coloring.ApplyColors(graph.Nodes,colors);
    try
    {
        graph.EnsureRightColoring();
    }
    catch (Exception ex)
    {
        System.Console.WriteLine(ex.Message);
    }
    // graph.Nodes.SetColorToAll(Color.Empty);
});

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
}, x => x.Position);
