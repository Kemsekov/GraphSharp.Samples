using GraphSharp;
using GraphSharp.Graphs;
using MathNet.Numerics.LinearAlgebra.Single;
using SixLabors.ImageSharp;
using SysColor = System.Drawing.Color;

var N = 20;
var node_probability = 0.6;
var edge_probability = 0.8;
var seed = 500;

System.Console.WriteLine("Creaing graph...");
Graph graph = CreatePercolationGraph(N, node_probability, edge_probability, seed);

System.Console.WriteLine("Creating Image...");
using var image = ImageSharpShapeDrawer.CreateImage(graph, drawer =>
    {
        drawer.Clear(SysColor.Black);
        drawer.DrawEdgesParallel(graph.Edges, 0.01, color: SysColor.Azure);
        drawer.DrawNodesParallel(graph.Nodes, 0.012, color: SysColor.Red);
        drawer.DrawNodeIds(graph.Nodes, SysColor.White, 0.01);
    },
    x => (Vector)(x.MapProperties().Position*0.9f+0.05f),
    outputResolution: 2000
);

System.Console.WriteLine("Saving Image...");
image.SaveAsJpeg("example.jpg");

static Graph CreatePercolationGraph(int N, double node_probability, double edge_probability, int seed)
{
    var graph = new Graph();
    graph.Do.CreateNodes(N * N);
    var rnd = new Random(seed);
    for (int i = 0; i < N; i++)
    {
        for (int j = 0; j < N; j++)
        {
            var pos = new DenseVector([ 1.0f * i / N, 1.0f * j / N]);
            graph.Nodes[i * N + j].MapProperties().Position = pos;
        }
    }

    for (int i = 0; i < N; i++)
    {
        for (int j = 0; j < N; j++)
        {
            var right_ind = (i + 1) * N + j;
            var bottom_ind = i * N + j + 1;

            var current = i * N + j;

            if (i + 1 < N && rnd.NextSingle() < edge_probability)
            {
                graph.Edges.Add(new Edge(current, right_ind));
            }

            if (j + 1 < N && rnd.NextSingle() < edge_probability)
            {
                graph.Edges.Add(new Edge(current, bottom_ind));
            }
        }
    }

    foreach (var n in graph.Nodes)
    {
        if (rnd.NextSingle() >= node_probability)
        {
            graph.Do.RemoveNodes(n.Id);
        }
    }

    return graph;
}