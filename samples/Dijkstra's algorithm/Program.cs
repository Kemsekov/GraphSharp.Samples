using System.Drawing;
using GraphSharp;
using GraphSharp.Edges;
using GraphSharp.Graphs;
using GraphSharp.Nodes;
//this program showing how to find the shortest path between two nodes
//by summing and comparing sum of visited path

Edge.DefaultColor = Color.DarkViolet;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation();
graph.Do.MakeUndirected();
var startNode = argz.node1 % graph.Nodes.Count;
var endNode = argz.node2 % graph.Nodes.Count;

var pathFinder = graph.Do.FindShortestPaths(startNode);

var path = pathFinder.GetPath(endNode) ?? new List<Node>();
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

