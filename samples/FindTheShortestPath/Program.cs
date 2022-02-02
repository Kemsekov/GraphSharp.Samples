using GraphSharp;
using GraphSharp.Nodes;
using GraphSharp.Graphs;

//this program showing how to find the shortest path betwen two nodes
//by summing and comparing sum of visited path

ArgumentsHandler argz = new("settings.json");

var nodes = CreateNodes(argz);

var startNode = nodes.Nodes[argz.node1 % nodes.Nodes.Count];
var endNode = nodes.Nodes[argz.node2 % nodes.Nodes.Count];

var pathFinder = new PathFinder(startNode);

var graph = new Graph(nodes);
graph.AddVisitor(pathFinder, startNode.Id);

FindPath(startNode, endNode, graph);

var path = pathFinder.GetPath(endNode) ?? new List<INode>();
var pathLength = pathFinder.GetPathLength(endNode);
var computedPath = Helpers.ComputePathLength(path, (n1, n2) => (float)(n1 as NodeXY).Distance(n2 as NodeXY));
Helpers.ValidatePath(path);
System.Console.WriteLine($"---Path length from sum of path nodes distances {computedPath}");
System.Console.WriteLine($"---Path length {pathLength}");
System.Console.WriteLine($"---Path nodes visited {path.Count}");

Helpers.CreateImage(nodes, path, argz);

void FindPath(INode startNode, INode endNode, IGraph graph)
{
    Helpers.MeasureTime(() =>
    {
        System.Console.WriteLine($"Trying to find path from Node {startNode.Id} to Node {endNode.Id}...");
        for (int i = 0; i < argz.steps; i++)
        {
            graph.Propagate();
            if (pathFinder.GetPath(endNode) is not null)
            {
                System.Console.WriteLine($"Path found at {i} step");
                break;
            }
        }
    });
}
NodesFactory CreateNodes(ArgumentsHandler argz)
{
    NodesFactory result = default;
    Helpers.MeasureTime(() =>
    {
        System.Console.WriteLine("Creating nodes...");
        var rand = new Random(argz.nodeSeed >= 0 ? argz.nodeSeed : new Random().Next());
        var conRand = new Random(argz.connectionSeed >= 0 ? argz.connectionSeed : new Random().Next());

        result = new NodesFactory(id => new NodeXY(id, rand.NextDouble(), rand.NextDouble()), (node, parent) => new NodeConnector(node, parent), conRand)
            .CreateNodes(argz.nodesCount)
            .ForEach()
            .ConnectToClosest(argz.minEdges, argz.maxEdges, (node1, node2) => (float)((NodeXY)node1).Distance((NodeXY)node2))
            .MakeUndirected();
    });
    return result;
}
