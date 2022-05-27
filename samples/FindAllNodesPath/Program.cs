using System.Drawing;
using GraphSharp;
using GraphSharp.Nodes;
using GraphSharp.Propagators;
using SixLabors.ImageSharp.Drawing.Processing;

/// this program shows how to find a tree that visits all nodes. (like spanning tree but not optimal)

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
var startNode = graph.Nodes[argz.node1 % graph.Nodes.Count];

var algorithm = new Algorithm(graph,argz.nodesCount);
algorithm.SetPosition(argz.node1);

FindPath(startNode, algorithm);

var path = (algorithm.Path ?? Enumerable.Empty<NodeXY>()).ToList();
graph.ValidatePath(path);
var pathLength = Helpers.ComputePathLength(path);
System.Console.WriteLine($"Path length {pathLength}");
System.Console.WriteLine($"Path nodes visited {path.Count}");

Helpers.CreateImage(argz,graph.Configuration,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(graph.Edges,argz.thickness);
    drawer.DrawNodes(graph.Nodes,argz.nodeSize);
    if (path?.Count > 0)
    {
        drawer.DrawPath(path,Color.Wheat,argz.thickness);
    }
});

void FindPath(NodeXY startNode, Algorithm algorithm)
{
    Helpers.MeasureTime(() =>
    {
        System.Console.WriteLine($"Trying to find path from {startNode} to visit all nodes");
        for (int i = 0; i < argz.steps; i++)
        {
            if (algorithm.PathDone)
            {
                System.Console.WriteLine($"Path done at {i} step");
                return;
            }
            algorithm.Propagate();
        }
    });
}


