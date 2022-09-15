using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
//this program showing how meet in the middle path finder work


ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulationWithoutHull();

var startNode = argz.node1 % graph.Nodes.Count;
var endNode = argz.node2 % graph.Nodes.Count;
IList<Node> path1 = new List<Node>();
IList<Node> path2 = new List<Node>();
IList<Node> path3 = new List<Node>();
IList<Node> path4 = new List<Node>();

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by meeting in the middle parallel (wheat)");
    path1= graph.Do.FindPathByMeetInTheMiddleParallel(startNode,endNode);
    System.Console.WriteLine($"Count of nodes in the path {path1.Count}");
    System.Console.WriteLine($"Path length {Helpers.ComputePathLength(path1)}");
    graph.ValidatePath(path1);
});

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by parallel any path finder (orange)");
    path2 = graph.Do.FindAnyPathParallel(startNode,endNode);
    System.Console.WriteLine($"Count of nodes in the path {path2.Count}");
    System.Console.WriteLine($"Path length {Helpers.ComputePathLength(path2)}");
    graph.ValidatePath(path2);
});

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by meet in the middle by parallel Dijkstra path finder (red)");
    path3 = graph.Do.FindPathByMeetInTheMiddleDijkstraParallel(startNode,endNode);
    System.Console.WriteLine($"Count of nodes in the path {path3.Count}");
    System.Console.WriteLine($"Path length {Helpers.ComputePathLength(path3)}");
    graph.ValidatePath(path3);
});

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by  parallel Dijkstra path finder (green)");
    path4 = graph.Do.FindShortestPathsDijkstra(startNode).GetPath(endNode);
    System.Console.WriteLine($"Count of nodes in the path {path4.Count}");
    System.Console.WriteLine($"Path length {Helpers.ComputePathLength(path4)}");
    graph.ValidatePath(path4);
});

Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
graph.Edges.SetColorToAll(Color.FromArgb(10,50,50));
Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawNode(graph.Nodes[startNode],argz.nodeSize);
    drawer.DrawNode(graph.Nodes[endNode],argz.nodeSize);
    if (path1?.Count > 0)
    {
        drawer.DrawPath(path1,Color.Wheat,argz.thickness);
    }
    if (path2?.Count > 0)
    {
        drawer.DrawPath(path2,Color.Orange,argz.thickness);
    }
    if (path3?.Count > 0)
    {
        drawer.DrawPath(path3,Color.Red,argz.thickness);
    }
    if (path4?.Count > 0)
    {
        drawer.DrawPath(path4,Color.Green,argz.thickness);
    }
});

