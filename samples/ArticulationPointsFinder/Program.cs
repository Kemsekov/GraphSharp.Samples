
using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;

ArgumentsHandler argz = new("settings.json");

var graph = CreateSampleGraph();
graph.Do.MakeDirected();
var points = graph.Do.FindArticulationPointsTarjan();
foreach (var p in points)
{
    //mark articulation points
    p.MapProperties().Color = Color.Aqua;
}

Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(graph.Edges, argz.thickness,Color.DarkViolet);
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes, Color.White, argz.fontSize);
}, x=>x.MapProperties().Position);

GraphSharp.Graphs.Graph CreateSampleGraph()
{
    var graph = Helpers.CreateGraph(argz);
    graph.Edges.Clear();

    graph.Do.CreateNodes(8);

    graph.Nodes[0].MapProperties().Position = DenseVector.OfArray([0.1f, 0.1f]);
    graph.Nodes[2].MapProperties().Position = DenseVector.OfArray([0.1f, 0.7f]);

    graph.Nodes[1].MapProperties().Position =  DenseVector.OfArray([0.3f, 0.5f]);
    graph.Nodes[6].MapProperties().Position =  DenseVector.OfArray([0.3f, 0.8f]);
    graph.Nodes[3].MapProperties().Position =  DenseVector.OfArray([0.7f, 0.2f]);
    graph.Nodes[4].MapProperties().Position =  DenseVector.OfArray([0.7f, 0.6f]);
    graph.Nodes[5].MapProperties().Position =  DenseVector.OfArray([0.9f, 0.6f]);
    graph.Nodes[7].MapProperties().Position =  DenseVector.OfArray([0.9f, 0.8f]);


    graph.Edges.Add(new(graph.Nodes[0], graph.Nodes[1]));

    graph.Edges.Add(new(graph.Nodes[2], graph.Nodes[0]));

    graph.Edges.Add(new(graph.Nodes[2], graph.Nodes[1]));

    graph.Edges.Add(new(graph.Nodes[6], graph.Nodes[1]));

    graph.Edges.Add(new(graph.Nodes[3], graph.Nodes[1]));

    graph.Edges.Add(new(graph.Nodes[1], graph.Nodes[4]));

    graph.Edges.Add(new(graph.Nodes[3], graph.Nodes[5]));

    graph.Edges.Add(new(graph.Nodes[5], graph.Nodes[4]));

    graph.Edges.Add(new(graph.Nodes[5], graph.Nodes[7]));

    return graph;
}