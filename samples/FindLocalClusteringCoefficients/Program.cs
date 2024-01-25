using System.Drawing;
using GraphSharp;
using MathNet.Numerics.LinearAlgebra.Single;

//bigger nodes will have bigger local clustering coefficient

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation(x=>x.MapProperties().Position);
graph.Do.MakeBidirected();
var coeffs = new double[0];
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding local clustering coefficients");
    using var l_coeffs = graph.Do.FindLocalClusteringCoefficients();
    coeffs=l_coeffs.ToArray();
});

coeffs.Select((value, index) =>
{
    var node = graph.Nodes[index];
    node.MapProperties().Weight = value;
    return 1;
}).ToArray();

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    foreach(var n in graph.Nodes)
        drawer.DrawNode(n, argz.nodeSize*n.MapProperties().Weight);
    drawer.DrawNodeIds(graph.Nodes, Color.Wheat, argz.fontSize);
},x=> (Vector)(x.MapProperties().Position*0.9f+0.05f));