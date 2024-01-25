using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
IEnumerable<Edge> tree = new List<Edge>();
var distance = (Node n1,Node n2)=>(double)(n1.MapProperties().Position-n2.MapProperties().Position).L2Norm();
graph.Do.DelaunayTriangulation(x=>x.MapProperties().Position);
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding minimal spanning tree...");
    tree = graph.Do.FindSpanningForestKruskal().Forest;
});
System.Console.WriteLine(graph.Nodes.Average(x=>graph.Edges.Degree(x.Id)));

foreach (var edge in tree)
{
    edge.Color = Color.Azure;
}
Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.MapProperties().Position,(n,p)=>n.MapProperties().Position = p);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    // drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawEdgesParallel(tree, argz.thickness);
},x=>x.MapProperties().Position);
