using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding components...");
    var componentsResult = graph.Do.FindComponents();
    System.Console.WriteLine($"Found {componentsResult.Components.Count()} components");
    foreach (var c in componentsResult.Components)
    {
        var color = Color.FromArgb(Random.Shared.Next(256), Random.Shared.Next(256), Random.Shared.Next(256));
        foreach (var n in c)
        {
            n.MapProperties().Color = color;
        }
    }
});
Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
},x=> (Vector)(x.MapProperties().Position*0.9f+0.05f));
