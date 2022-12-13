using System.Drawing;
using GraphSharp;
using GraphSharp.Common;
using GraphSharp.Graphs;
//this program showing how to find the shortest path between two nodes
//by summing and comparing sum of visited path


ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation(x=>x.Position);

var startNode = argz.node1 % graph.Nodes.Count;
var endNode = argz.node2 % graph.Nodes.Count;
IPath<Node> path = default;
double pathLength = 0;

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding all shortest paths parallel...");
    var shortestPathFinder = graph.Do.FindShortestPathsParallelDijkstra(startNode,x=>x.Weight);
    path = shortestPathFinder.GetPath(endNode);
    pathLength = shortestPathFinder.PathLength[endNode];
    System.Console.WriteLine($"Shortest path length : {pathLength}");
});
Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding all shortest paths...");
    var shortestPathFinder = graph.Do.FindShortestPathsDijkstra(startNode,x=>x.Weight);
    path = shortestPathFinder.GetPath(endNode);
    pathLength = shortestPathFinder.PathLength[endNode];
    System.Console.WriteLine($"Shortest path length : {pathLength}");
});
graph.ValidatePath(path);


Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n,p)=>n.Position = p);
Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    if (path?.Count > 0)
    {
        drawer.DrawPath(path,Color.Wheat,argz.thickness);
    }
},x=>x.Position);

