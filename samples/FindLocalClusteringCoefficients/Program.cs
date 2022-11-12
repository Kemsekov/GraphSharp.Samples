using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using GraphSharp;
using GraphSharp.Common;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using MathNet.Numerics.LinearAlgebra.Single;
using SampleBase;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation(x=>x.Position);
graph.Do.MakeBidirected();
var coeffs = new double[0];
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding local clustering coefficients");
    coeffs = graph.Do.FindLocalClusteringCoefficients();
});
coeffs.Select((value, index) =>
{
    var node = graph.Nodes[index];
    node.Weight = value;
    return 1;
}).ToArray();

Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n,p)=>n.Position = p);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    foreach(var n in graph.Nodes)
        drawer.DrawNode(n, argz.nodeSize*n.Weight);
    drawer.DrawNodeIds(graph.Nodes, Color.Wheat, argz.fontSize);
},x=>x.Position);