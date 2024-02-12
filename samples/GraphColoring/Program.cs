
using System.Drawing;
using GraphSharp;
using GraphSharp.Adapters;
using GraphSharp.Graphs;
using QuikGraph.Algorithms.VertexColoring;

// I am very proud of colorings in my library

void ColoringInformation(ColoringResult coloring,Graph graph, Color[]? colors){
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
}

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
if (graph.Edges.Count == 0)
    graph.Do.DelaunayTriangulation(x => x.MapProperties().Position);

var colors =
    new[] { Color.Azure, Color.Yellow, Color.Red, Color.Coral, Color.Blue, Color.Violet, Color.Aqua,Color.Green,Color.Orange,Color.Brown,Color.Aquamarine };

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Greedy coloring graph...");
    var coloring = graph.Do.GreedyColorNodes();

    graph.Nodes.SetColorToAll(Color.Empty);
});

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("DSatur coloring graph...");
    var coloring = graph.Do.DSaturColorNodes();
    ColoringInformation(coloring,graph,colors);
    graph.Nodes.SetColorToAll(Color.Empty);
});

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("QuikGraph coloring graph...");
    var coloring = graph.Do.QuikGraphColorNodes();
    ColoringInformation(coloring,graph,colors);
    graph.Nodes.SetColorToAll(Color.Empty);
});

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Recursive largest first (RLF) coloring graph...");
    var coloring = graph.Do.RLFColorNodes();
    ColoringInformation(coloring,graph,colors);
    graph.Nodes.SetColorToAll(Color.Empty);

});

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("SAT coloring graph with 6 colors...");
    var coloring = graph.Do.SATColoring(6,out var satResult);
    System.Console.WriteLine("Coloring: " +satResult);
    ColoringInformation(coloring,graph,colors);
});

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
}, x => x.MapProperties().Position);
