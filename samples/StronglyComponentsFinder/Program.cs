using System.Diagnostics;
using System.Drawing;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using MathNet.Numerics.LinearAlgebra.Single;
using SampleBase;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
var components = graph.Do.FindStronglyConnectedComponentsTarjan();
System.Console.WriteLine(components.Sum(x=>x.nodes.Count()));
System.Console.WriteLine(components.Count());
foreach(var c in components){
    var color = Color.FromArgb(Random.Shared.Next(256),Random.Shared.Next(256),Random.Shared.Next(256));
    foreach(var n in c.nodes){
        n.Color = color;
    }
}

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawDirectionsParallel(graph.Edges,argz.thickness,argz.directionLength,Color.Orange);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
},x=>x.Position);