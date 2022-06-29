
using System.Drawing;

ArgumentsHandler argz = new("settings.json");

var graph = CreateSampleGraph();

var points = graph.Do.GetArticulationPoints();
foreach (var p in points)
{
    //mark articulation points
    p.Color = Color.Aqua;
}

Helpers.CreateImage(argz, graph.Configuration, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(graph.Edges, argz.thickness);
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes, Color.White, argz.fontSize);
});

GraphSharp.GraphStructures.GraphStructure<NodeXY, NodeConnector> CreateSampleGraph()
{
    var graph = Helpers.CreateGraph(argz);
    graph.Edges.Clear();

    graph.Create(8);

    graph.Nodes[0].Position = new(0.1f, 0.1f);
    graph.Nodes[2].Position = new(0.1f, 0.7f);

    graph.Nodes[1].Position = new(0.3f, 0.5f);
    graph.Nodes[6].Position = new(0.3f, 0.8f);

    graph.Nodes[3].Position = new(0.7f, 0.2f);
    graph.Nodes[4].Position = new(0.7f, 0.6f);

    graph.Nodes[5].Position = new(0.9f, 0.6f);

    graph.Nodes[7].Position = new(0.9f, 0.8f);


    graph.Edges.Add(new(graph.Nodes[0], graph.Nodes[1]));
    graph.Edges.Add(new(graph.Nodes[1], graph.Nodes[0]));

    graph.Edges.Add(new(graph.Nodes[2], graph.Nodes[0]));
    graph.Edges.Add(new(graph.Nodes[0], graph.Nodes[2]));

    graph.Edges.Add(new(graph.Nodes[2], graph.Nodes[1]));
    graph.Edges.Add(new(graph.Nodes[1], graph.Nodes[2]));

    graph.Edges.Add(new(graph.Nodes[6], graph.Nodes[1]));
    graph.Edges.Add(new(graph.Nodes[1], graph.Nodes[6]));

    graph.Edges.Add(new(graph.Nodes[3], graph.Nodes[1]));
    graph.Edges.Add(new(graph.Nodes[1], graph.Nodes[3]));

    graph.Edges.Add(new(graph.Nodes[1], graph.Nodes[4]));
    graph.Edges.Add(new(graph.Nodes[4], graph.Nodes[1]));

    graph.Edges.Add(new(graph.Nodes[3], graph.Nodes[5]));
    graph.Edges.Add(new(graph.Nodes[5], graph.Nodes[3]));

    graph.Edges.Add(new(graph.Nodes[5], graph.Nodes[4]));
    graph.Edges.Add(new(graph.Nodes[4], graph.Nodes[5]));

    graph.Edges.Add(new(graph.Nodes[5], graph.Nodes[7]));

    return graph;
}