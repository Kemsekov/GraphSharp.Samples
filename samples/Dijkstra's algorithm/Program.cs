using System.Drawing;
using GraphSharp;
using GraphSharp.GraphStructures;
//this program showing how to find the shortest path between two nodes
//by summing and comparing sum of visited path

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);

var startNode = graph.Nodes[argz.node1 % graph.Nodes.Count];
var endNode = graph.Nodes[argz.node2 % graph.Nodes.Count];

var pathFinder = new Algorithm(startNode,graph);
pathFinder.SetPosition(startNode.Id);

FindPath(startNode, endNode, pathFinder);

var path = pathFinder.GetPath(endNode) ?? new List<NodeXY>();
var pathLength = pathFinder.GetPathLength(endNode);
var computedPath = Helpers.ComputePathLength(path);
graph.ValidatePath(path);

System.Console.WriteLine($"---Path length from sum of path nodes distances {computedPath}");
System.Console.WriteLine($"---Path length {pathLength}");
System.Console.WriteLine($"---Path nodes visited {path.Count}");

Helpers.ShiftNodesToFitInTheImage(graph.Nodes);

Helpers.CreateImage(argz,graph.Configuration,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
    if (path?.Count > 0)
    {
        drawer.DrawPath(path,Color.Wheat,argz.thickness);
    }
});

void FindPath(NodeXY startNode, NodeXY endNode, Algorithm algorithm)
{
    Helpers.MeasureTime(() =>
    {
        System.Console.WriteLine($"Trying to find path from Node {startNode.Id} to Node {endNode.Id}...");
        for (int i = 0; i < argz.steps; i++)
        {
            algorithm.Propagate();
            if (pathFinder.GetPath(endNode) is not null)
            {
                System.Console.WriteLine($"Path found at {i} step");
                break;
            }
        }
    });
}

