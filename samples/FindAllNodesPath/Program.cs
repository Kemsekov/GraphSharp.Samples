using GraphSharp;
using GraphSharp.Nodes;
using GraphSharp.Propagators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

/// this program shows how to find a tree that visits all nodes. (like spanning tree but not optimal)

ArgumentsHandler argz = new("settings.json");

var nodes = Helpers.CreateNodes(argz);
var startNode = nodes.Nodes[argz.node1 % nodes.Nodes.Count];

var algorithm = new Algorithm(argz.nodesCount);
algorithm.SetNodes(nodes);
algorithm.SetPosition(argz.node1);

FindPath(startNode, algorithm);

var path = (algorithm.Path ?? Enumerable.Empty<NodeXY>()).ToList();
Helpers.ValidatePath(path);
var pathLength = Helpers.ComputePathLength(path,(n1,n2)=>
        (float)(n1 as NodeXY ?? throw new Exception("bad type")).Distance(n2 as NodeXY ?? throw new Exception("bad type")));
System.Console.WriteLine($"Path length {pathLength}");
System.Console.WriteLine($"Path nodes visited {path.Count}");

Helpers.CreateImage(argz,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(nodes.Nodes);
    drawer.DrawNodes(nodes.Nodes);
    if (path?.Count > 0)
    {
        drawer.DrawPath(path,Color.Wheat);
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


