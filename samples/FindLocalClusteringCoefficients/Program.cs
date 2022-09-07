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
graph.Do.DelaunayTriangulation();
graph.Do.MakeUndirected();
var coeffs = new float[0];
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding local clustering coefficients");
    coeffs = graph.Do.FindLocalClusteringCoefficients();
});
coeffs.Select((value, index) =>
{
    // System.Console.WriteLine($"{index}\t{value}"); 
    var node = graph.Nodes[index];
    var color = node.Color;
    node.Color = Color.FromArgb((int)(value * 255), color.G, color.B);
    if(value==1)
        node.Color = Color.Orange;
    if(value==0)
        node.Color = Color.Blue;
    return 1;
}).ToArray();

Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
if(false)
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes, Color.Wheat, argz.fontSize);
});
argz.nodesCount += 10000;