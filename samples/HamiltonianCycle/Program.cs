using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
if (graph.Edges.Count == 0)
    graph.Do.DelaunayTriangulation(x => x.Position);

IList<Edge> b = new List<Edge>();

Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Ham cycle");

        var path = graph.Do.TryFindHamiltonianCycleByBubbleExpansion();
        graph.ValidateCycle(path);
        graph.ConvertPathToEdges(path.Path, out b);
        graph.Nodes.SetColorToAll(Color.Aqua);
        System.Console.WriteLine("Length of cycle is " + path.Cost);
        foreach (var t in path)
        {
            t.Color = Color.Empty;
        }
  
});

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawEdgesParallel(b, argz.thickness, Color.Orange);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
}, x => x.Position);