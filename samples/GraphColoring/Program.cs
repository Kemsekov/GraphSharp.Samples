
using System.Drawing;
using GraphSharp.Adapters;
using GraphSharp.Graphs;
using QuikGraph.Algorithms.VertexColoring;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
// graph.Do.DelaunayTriangulation();

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

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("QuikGraph coloring graph...");
    var quikGraph = graph.Converter.ToQuikGraph();
    var undirected = new QuikGraph.UndirectedBidirectionalGraph<GraphSharp.Node, EdgeAdapter<GraphSharp.Node, GraphSharp.Edge>>(quikGraph);
    var coloring = new VertexColoringAlgorithm<GraphSharp.Node, EdgeAdapter<GraphSharp.Node, GraphSharp.Edge>>(undirected);
    coloring.Compute();
    var colorsUsed = coloring.Colors.Select(x=>x.Value).Distinct().Count();
    System.Console.WriteLine($"Total colors used : {colorsUsed}");
    var usedColors = coloring.Colors.GroupBy(x=>x.Value).ToDictionary(x=>x.First().Value ?? throw new Exception());
    System.Console.WriteLine("ColorId\tNodesColored");
    foreach (var colorInfo in usedColors)
    {
        System.Console.Write(colorInfo.Key);
        System.Console.Write("\t");
        System.Console.Write(colorInfo.Value.Count());
        System.Console.WriteLine();
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x => x.Value.Count()));
});

graph.EnsureRightColoring();
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(graph.Edges, argz.thickness);
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
});
