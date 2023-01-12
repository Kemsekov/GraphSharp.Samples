using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
if (graph.Edges.Count == 0)
    graph.Do.ConnectAsHamiltonianCycle(x => x.Position);
var rand = graph.Configuration.Rand;
for (int i = 0; i < 50; i++)
{
    var n1 = graph.Nodes[rand.Next(graph.Nodes.MaxNodeId + 1)];
    var n2 =
        graph.Nodes
        .Where(x => x.Id != n1.Id)
        .Where(x => graph.Edges.EdgesBetweenNodes(n1.Id, x.Id).Count() == 0)
        .MinBy(x => (x.Position - n1.Position).Length());
    if(n2 is null) continue;
    var edge = graph.Configuration.CreateEdge(n1,n2);
    // edge.Color = Color.Brown;
    graph.Edges.Add(edge);

}
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
    drawer.DrawEdgesParallel(graph.Edges.OrderBy(x=>x.Color==Color.Brown ? 0 : 1), argz.thickness);
    drawer.DrawEdgesParallel(b, argz.thickness, Color.Orange);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
}, x => x.Position);