using System.Drawing;
using GraphSharp;
using GraphSharp.Common;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
//this program showing how meet in the middle path finder work


ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulationWithoutHull(x=>x.Position);

var startNode = argz.node1 % graph.Nodes.Count;
var endNode = argz.node2 % graph.Nodes.Count;
IList<Node> path1 = new List<Node>();
IList<Node> path2 = new List<Node>();
IList<Node> path3 = new List<Node>();
IList<Node> path4 = new List<Node>();

var pathType = PathType.InEdges;

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by parallel any path finder (orange)");
    var p = graph.Do.FindAnyPathParallel(startNode,endNode,pathType: pathType);
    path2 = p.Path.ToList();
    System.Console.WriteLine($"Count of nodes in the path {path2.Count}");
    System.Console.WriteLine($"Path length {p.Cost}");
    graph.ValidatePath(p);
});

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by meeting in the middle parallel (wheat)");
    var p = graph.Do.FindPathByMeetInTheMiddleParallel(startNode,endNode,undirected:false);
    path1 = p.Path.ToList();
    System.Console.WriteLine($"Count of nodes in the path {path1.Count}");
    System.Console.WriteLine($"Path length {p.Cost}");
    graph.ValidatePath(p);
});


Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by meet in the middle by parallel Dijkstra path finder (red)");
    var p = graph.Do.FindPathByMeetInTheMiddleDijkstraParallel(startNode,endNode, undirected:false);
    path3 = p.Path.ToList();
    System.Console.WriteLine($"Count of nodes in the path {path3.Count}");
    System.Console.WriteLine($"Path length {p.Cost}");
    graph.ValidatePath(p);
});

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by  parallel Dijkstra path finder (green)");
    var p = graph.Do.FindShortestPathsDijkstra(startNode,pathType:pathType).GetPath(endNode);
    path4 = p.Path;
    System.Console.WriteLine($"Count of nodes in the path {path4.Count}");
    System.Console.WriteLine($"Path length {p.Cost}");
    graph.ValidatePath(p);
});

Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n,p)=>n.Position = p);
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
},x=>x.Position);

