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

System.Console.WriteLine($"Nodes {graph.Nodes.Count()}");
System.Console.WriteLine($"Edges {graph.Edges.Count()}");

var colors =
    new[] { Color.Yellow, Color.Red, Color.Coral, Color.Blue, Color.Violet, Color.Aqua,Color.Green,Color.Orange,Color.Brown,Color.Aquamarine };

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Greedy coloring graph...");
    var coloring = graph.Do.GreedyColorNodes();
    ColoringInformation(coloring,graph,colors);

    graph.Nodes.SetColorToAll(Color.Empty);
});

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("DSatur coloring graph...");
    var coloring = graph.Do.DSaturColorNodes();
    ColoringInformation(coloring,graph,colors);
    graph.Nodes.SetColorToAll(Color.Empty);
});

// Helpers.MeasureTime(() =>
// {
//     System.Console.WriteLine("QuikGraph coloring graph...");
//     var coloring = graph.Do.QuikGraphColorNodes();
//     ColoringInformation(coloring,graph,colors);
//     graph.Nodes.SetColorToAll(Color.Empty);
// });

// Helpers.MeasureTime(() =>
// {
//     System.Console.WriteLine("Recursive largest first (RLF) coloring graph...");
//     var coloring = graph.Do.RLFColorNodes();
//     ColoringInformation(coloring,graph,colors);
//     graph.Nodes.SetColorToAll(Color.Empty);
// });

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("SAT coloring graph...");
    foreach(var colors_count in new[]{2,3,4,5,6,7,8,9,10,11,12,13,14,15}){
        var coloring = graph.Do.SATColoring(colors_count,out var satResult);
        if(coloring.CountUsedColors().Keys.Count()!=colors_count){
            continue;
        }
        System.Console.WriteLine("Coloring: " +satResult);
        ColoringInformation(coloring,graph,colors);
        break;
    }
});

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.White);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness,Color.Black);
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
}, x => x.MapProperties().Position);
