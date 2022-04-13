using GraphSharp;
using GraphSharp.GraphStructures;
using SixLabors.ImageSharp;
//this program showing how to find the shortest path betwen two nodes
//by summing and comparing sum of visited path

ArgumentsHandler argz = new("settings.json");

var nodes = Helpers.CreateGraph(argz);

var startNode = nodes.Nodes[argz.node1 % nodes.Nodes.Count];
var endNode = nodes.Nodes[argz.node2 % nodes.Nodes.Count];

var pathFinder = new Algorithm(startNode);
pathFinder.SetNodes(nodes);
pathFinder.SetPosition(startNode.Id);

FindPath(startNode, endNode, pathFinder);

var path = pathFinder.GetPath(endNode) ?? new List<NodeXY>();
var pathLength = pathFinder.GetPathLength(endNode);
var computedPath = Helpers.ComputePathLength(
    path, 
    (n1, n2) => 
        (float)(n1 as NodeXY ?? throw new Exception("bad type")).Distance(n2 as NodeXY ?? throw new Exception("bad type")));
Helpers.ValidatePath(path);
System.Console.WriteLine($"---Path length from sum of path nodes distances {computedPath}");
System.Console.WriteLine($"---Path length {pathLength}");
System.Console.WriteLine($"---Path nodes visited {path.Count}");

Helpers.CreateImage(argz,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(nodes.Nodes);
    drawer.DrawNodes(nodes.Nodes);
    if (path?.Count > 0)
    {
        drawer.DrawPath(path,Color.Wheat);
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

