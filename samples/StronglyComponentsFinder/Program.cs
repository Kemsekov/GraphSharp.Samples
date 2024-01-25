using System.Drawing;
using GraphSharp;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
var scc = graph.Do.FindStronglyConnectedComponentsTarjan();
foreach(var c in scc.Components){
    var color = Color.FromArgb(Random.Shared.Next(256),Random.Shared.Next(256),Random.Shared.Next(256));
    foreach(var n in c.nodes){
        n.MapProperties().Color = color;
    }
}

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawDirectionsParallel(graph.Edges,argz.thickness,argz.directionLength,Color.Orange);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
},x=>x.MapProperties().Position);