using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Nodes;
using GraphSharp.Propagators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

/// this program shows how to find path that visits all nodes

ArgumentsHandler argz = new("settings.json");

var nodes = Helpers.CreateNodes(argz);
var startNode = nodes.Nodes[argz.node1 % nodes.Nodes.Count];

var visitor = new AllPathFinder(argz.nodesCount);
var graph = new Graph(nodes, PropagatorFactory.SingleThreaded());
graph.AddVisitor(visitor, argz.node1);

FindPath(startNode, graph);

var path = (visitor.Path ?? Enumerable.Empty<INode>()).ToList();
Helpers.ValidatePath(path);
var pathLength = Helpers.ComputePathLength(path,(n1,n2)=>
        (float)(n1 as NodeXY ?? throw new Exception("bad type")).Distance(n2 as NodeXY ?? throw new Exception("bad type")));
System.Console.WriteLine($"Path length {pathLength}");
System.Console.WriteLine($"Path nodes visited {path.Count}");

Helpers.CreateImage(nodes, path, argz);

void FindPath(INode startNode, IGraph graph)
{
    Helpers.MeasureTime(() =>
    {
        System.Console.WriteLine($"Trying to find path from {startNode} to visit all nodes");
        for (int i = 0; i < argz.steps; i++)
        {
            if (visitor.PathDone)
            {
                System.Console.WriteLine($"Path done at {i} step");
                return;
            }
            graph.Propagate();
        }
    });
}


