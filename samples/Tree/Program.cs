using System.Drawing;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation();
IList<NodeConnector> tree = new List<NodeConnector>();
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding minimal spanning tree...");
    tree = graph.Do.FindSpanningTree();
});
foreach (var edge in tree)
{
    edge.Color = Color.Azure;
}
Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz, graph.Configuration, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawEdgesParallel(tree, argz.thickness);
});
