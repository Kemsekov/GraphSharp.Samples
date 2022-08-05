using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
//this program showing how to find the shortest path between two nodes
//by summing and comparing sum of visited path


ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
// graph.Do.DelaunayTriangulation();
graph.Do.MakeUndirected();
graph.Edges.SetColorToAll(Color.DarkViolet);
graph.Nodes.SetColorToAll(Color.Brown);
var startNode = argz.node1 % graph.Nodes.Count;
var endNode = argz.node2 % graph.Nodes.Count;

var shortestPathFinder = graph.Do.FindShortestPathsParallel(startNode,x=>x.Weight);

var path1 = shortestPathFinder.GetPath(endNode) ?? new List<Node>();
var pathLength1 = shortestPathFinder.GetPathLength(endNode);


graph.ValidatePath(path1);

System.Console.WriteLine($"Shortest path length : {pathLength1}");

Helpers.ShiftNodesToFitInTheImage(graph.Nodes);

Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
    if (path1?.Count > 0)
    {
        drawer.DrawPath(path1,Color.Wheat,argz.thickness);
    }
});

